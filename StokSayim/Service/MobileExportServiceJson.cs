using System;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StokSayim.Service
{
    public class MobileExportServiceJson
    {
        private readonly string _connectionString;

        public MobileExportServiceJson(string connectionString)
        {
            _connectionString = connectionString;
        }

        // JSON olarak dönüş yapacak ana metod
        public Dictionary<string, object> ExportSayimToJson(int sayimId)
        {
            var exportResults = new Dictionary<string, object>();

            try
            {
                // Sayım bilgisini al
                var sayimInfo = Global.SayimRepository.GetByIdWithRelations(sayimId);
                if (sayimInfo == null)
                {
                    throw new Exception($"Sayım ID {sayimId} bulunamadı.");
                }

                int brandId = sayimInfo.BrandID;

                // Her tablo için JSON export metodlarını çağır
                exportResults["Parameters"] = GetParametersJson(sayimInfo);
                exportResults["SayimPersonel"] = ExportSayimPersonelToJson(sayimId);
                exportResults["SayimLokasyon"] = ExportSayimLokasyonToJson(sayimId);
                exportResults["SayimLokasyonDetay"] = ExportSayimLokasyonDetayToJson(sayimId);
                exportResults["CatalogItems"] = ExportCatalogItemsToJson(brandId);

                return exportResults;
            }
            catch (Exception ex)
            {
                exportResults["Error"] = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return exportResults;
            }
        }

        private object GetParametersJson(dynamic sayimInfo)
        {
            return new
            {
                SayimId = sayimInfo.Id,
                SayimKodu = sayimInfo.SayimKodu,
                BrandId = sayimInfo.BrandID,
                BrandName = sayimInfo.Brand?.BrandName,
                StoreId = sayimInfo.StoreID,
                StoreName = sayimInfo.Store?.StoreName,
                ExportDate = DateTime.Now
            };
        }

        private List<object> ExportSayimPersonelToJson(int sayimId)
        {
            using (var connection = new SqlConnection(Global.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM SayimPersonel WHERE SayimId = @SayimId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    var personelList = new List<object>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            personelList.Add(new
                            {
                                Id = reader["Id"],
                                SayimId = reader["SayimId"],
                                TcNo = reader["TcNo"],
                                Adi = reader["Adi"],
                                Soyadi = reader["Soyadi"],
                                Tip = reader["Tip"]
                            });
                        }
                    }

                    return personelList;
                }
            }
        }

        private List<object> ExportSayimLokasyonToJson(int sayimId)
        {
            using (var connection = new SqlConnection(Global.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM SayimLokasyon WHERE SayimId = @SayimId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    var lokasyonList = new List<object>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lokasyonList.Add(new
                            {
                                Id = reader["Id"],
                                Adi = reader["Adi"],
                                AlanKod = reader["AlanKod"],
                                Aciklama = reader["Aciklama"],
                                Miktar = reader["Miktar"],
                                SayimId = reader["SayimId"],
                                Aktif = reader["Aktif"],
                                IptalAciklama = reader["IptalAciklama"]
                            });
                        }
                    }

                    return lokasyonList;
                }
            }
        }

        private List<object> ExportSayimLokasyonDetayToJson(int sayimId)
        {
            using (var connection = new SqlConnection(Global.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM SayimLokasyonDetay WHERE SayimId = @SayimId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    var detayList = new List<object>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detayList.Add(new
                            {
                                Id = reader["Id"],
                                SayimId = reader["SayimId"],
                                AlanKod = reader["AlanKod"],
                                LokasyonKod = reader["LokasyonKod"],
                                SayimLokasyonId = reader["SayimLokasyonId"],
                                Aktif = reader["Aktif"],
                                IptalAciklama = reader["IptalAciklama"]
                            });
                        }
                    }

                    return detayList;
                }
            }
        }

        private List<object> ExportCatalogItemsToJson(int brandId)
        {
            using (var connection = new SqlConnection(Global.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM CatalogItems WHERE BrandID = @BrandID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BrandID", brandId);

                    var catalogList = new List<object>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            catalogList.Add(new
                            {
                                Barcode = reader["Barcode"],
                                Description = reader["Description"],
                                Category = reader["Category"],
                                PrivateCode = reader["PrivateCode"]
                            });
                        }
                    }

                    return catalogList;
                }
            }
        }

        public void SaveJsonToFile(Dictionary<string, object> exportedData, string outputPath, string sayimKodu)
        {
            // SayimKodu'nu kullanarak alt klasör oluştur
            string sayimKoduFolder = Path.Combine(outputPath, sayimKodu);

            // Klasör yoksa oluştur
            if (!Directory.Exists(sayimKoduFolder))
            {
                Directory.CreateDirectory(sayimKoduFolder);
            }

            foreach (var entry in exportedData)
            {
                if (entry.Key != "Error")
                {
                    string filePath = Path.Combine(sayimKoduFolder, $"{entry.Key}.json");
                    string jsonContent = JsonConvert.SerializeObject(entry.Value, Formatting.Indented);
                    File.WriteAllText(filePath, jsonContent);
                }
            }

            // Hata varsa ayrıca kaydet
            if (exportedData.ContainsKey("Error"))
            {
                string errorPath = Path.Combine(sayimKoduFolder, "error.json");
                string errorJson = JsonConvert.SerializeObject(exportedData["Error"], Formatting.Indented);
                File.WriteAllText(errorPath, errorJson);
            }
        }


        public void ExportAndSaveToJson(int sayimId, string outputPath)
        {
            var exportedData = ExportSayimToJson(sayimId);

            // SayimKodu'nu al
            var sayimInfo = Global.SayimRepository.GetByIdWithRelations(sayimId);
            string sayimKodu = sayimInfo?.SayimKodu;

            if (string.IsNullOrEmpty(sayimKodu))
            {
                // SayimKodu yoksa, varsayılan bir değer kullan veya hata fırlat
                sayimKodu = $"Sayim_{sayimId}";
            }

            SaveJsonToFile(exportedData, outputPath, sayimKodu);
        }
    }
}
