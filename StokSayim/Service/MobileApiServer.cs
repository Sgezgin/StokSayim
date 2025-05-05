using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace StokSayim.Service
{
    public class MobileApiServer
    {
        private HttpListener _listener;
        private string _exportFolderPath;

        public MobileApiServer(string exportFolderPath, int port = 8080)
        {
            _exportFolderPath = exportFolderPath;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"API Sunucusu çalışıyor. Klasör: {_exportFolderPath}");

            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                ProcessRequest(context);
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // CORS ve güvenlik ayarları
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                // Rota kontrolü
                if (request.Url.LocalPath == "/database")
                {
                    SendDatabaseFile(response);
                }
                else if (request.Url.LocalPath == "/parameters")
                {
                    SendParametersFile(response);
                }
                else
                {
                    SendErrorResponse(response, 404, "Endpoint bulunamadı");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        private void SendDatabaseFile(HttpListenerResponse response)
        {
            try
            {
                string dbPath = Path.Combine(_exportFolderPath, "StkSayimDb.db");
                if (!File.Exists(dbPath))
                {
                    SendErrorResponse(response, 404, "Veritabanı dosyası bulunamadı");
                    return;
                }

                // SQLite dosyasının geçerli olduğunu kontrol et
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // Tablo sayısını doğrula
                    using (var cmd = new SQLiteCommand("SELECT count(*) FROM sqlite_master WHERE type='table'", connection))
                    {
                        int tableCount = Convert.ToInt32(cmd.ExecuteScalar());
                        Console.WriteLine($"Veritabanında {tableCount} tablo bulundu");
                    }

                    connection.Close();
                }

                response.ContentType = "application/x-sqlite3";
                response.AddHeader("Content-Disposition", "attachment; filename=StkSayimDb.db");

                // Arabellek kullanımını optimize et
                using (var fileStream = new FileStream(dbPath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.OutputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanı gönderme hatası: {ex.Message}");
                SendErrorResponse(response, 500, $"İç sunucu hatası: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        private void SendParametersFile(HttpListenerResponse response)
        {
            string paramPath = Path.Combine(_exportFolderPath, "parameters.json");

            if (!File.Exists(paramPath))
            {
                SendErrorResponse(response, 404, "Parametre dosyası bulunamadı");
                return;
            }

            response.ContentType = "application/json";

            using (var fileStream = File.OpenRead(paramPath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string jsonContent = reader.ReadToEnd();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            response.Close();
        }


        private void SendErrorResponse(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";

            var errorResponse = new
            {
                error = message,
                status = statusCode
            };

            string jsonError = JsonConvert.SerializeObject(errorResponse);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonError);

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
