using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using StokSayim.Helpers;
using System.Collections.Generic;

namespace StokSayim.Services
{
    /// <summary>
    /// SQLite veritabanı oluşturma ve mobil cihaza aktarma işlemlerini yöneten sınıf
    /// </summary>
    public class MobileExportService
    {
        private readonly string _connectionString;
        private readonly IProgress<int> _progress;

        /// <summary>
        /// Yeni bir MobileExportService örneği oluşturur
        /// </summary>
        /// <param name="connectionString">SQL Server bağlantı dizesi</param>
        /// <param name="progressBar">İlerleme çubuğu (isteğe bağlı)</param>
        public MobileExportService(string connectionString, Progress<int> progress = null)
        {
            _connectionString = connectionString;
            _progress = progress;
        }

        /// <summary>
        /// Sayım verilerini SQLite veritabanına aktarır ve mobil cihaza gönderilmesi için hazırlar
        /// </summary>
        /// <param name="sayimId">Aktarılacak sayım ID'si</param>
        /// <returns>İşlemin başarılı olup olmadığı</returns>
        public bool ExportSayimToMobile(int sayimId)
        {
            try
            {
                // Geçici klasör oluştur
                string tempFolder = Path.Combine(Path.GetTempPath(), "StokSayimExport");

                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                // SQLite veritabanı oluştur ve verileri aktar
                if (!ExportDatabaseToSQLite(sayimId, tempFolder))
                    return false;

                // Manuel kopyalama için kullanıcıya talimatlar ver
                string dbPath = Path.Combine(tempFolder, "StkSayimDb.db");
                string parametersJsonPath = Path.Combine(tempFolder, "parameters.json");

                // Dosyaların oluşturulduğunu kontrol et
                if (!File.Exists(dbPath) || !File.Exists(parametersJsonPath))
                {
                    MessageBox.Show("Kaynak dosyalar oluşturulamadı!", "Hata",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Windows Explorer'da dosyaları göster
                Process.Start("explorer.exe", $"/select,\"{dbPath}\"");

                MessageBox.Show(
                    "Lütfen aşağıdaki adımları izleyin:\n\n" +
                    "1. Görüntülenen klasörde iki dosya var: 'StkSayimDb.db' ve 'parameters.json'\n" +
                    "2. HT330 cihazını açın\n" +
                    "3. Cihazda 'STK' adında bir klasör oluşturun (yoksa)\n" +
                    "4. Bu dosyayı seçin ve 'STK' klasörüne kopyalayın\n\n" +
                    "Dosyalar başarıyla cihaza kopyalandıktan sonra uygulamayı başlatabilirsiniz.",
                    "Manuel Kopyalama Talimatları",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri aktarımı sırasında hata: {ex.Message}",
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Sayım verilerini SQLite veritabanına aktarır
        /// </summary>
        private bool ExportDatabaseToSQLite(int sayimId, string outputPath)
        {
            try
            {
                // SQLite bağlantısı oluştur
                string dbPath = Path.Combine(outputPath, "StkSayimDb.db");

                // Varsa dosyayı sil
                if (File.Exists(dbPath))
                    File.Delete(dbPath);

                // SQLite Connection oluştur
                SQLiteConnection.CreateFile(dbPath);
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // 1. Sayım bilgisini al
                    var sayimInfo = Global.SayimRepository.GetByIdWithRelations(sayimId);
                    if (sayimInfo == null)
                    {
                        MessageBox.Show($"Sayım ID {sayimId} bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    int brandId = sayimInfo.BrandID;
                    int totalProgress = 0;

                    // 2. Tabloları oluştur
                    CreateTables(connection);

                    // 3. CatalogItems tablosunu doldur
                    totalProgress++;
                    UpdateProgress(totalProgress, 4, 0);
                    ImportCatalogItems(connection, brandId, (progress) => UpdateProgress(totalProgress, 4, progress));

                    // 4. SayimLokasyon tablosunu doldur
                    totalProgress++;
                    UpdateProgress(totalProgress, 4, 0);
                    ImportSayimLokasyon(connection, sayimId, (progress) => UpdateProgress(totalProgress, 4, progress));

                    // 5. SayimLokasyonDetay tablosunu doldur
                    totalProgress++;
                    UpdateProgress(totalProgress, 4, 0);
                    ImportSayimLokasyonDetay(connection, sayimId, (progress) => UpdateProgress(totalProgress, 4, progress));

                    // 6. SayimPersonel tablosunu doldur
                    totalProgress++;
                    UpdateProgress(totalProgress, 4, 0);
                    ImportSayimPersonel(connection, sayimId, (progress) => UpdateProgress(totalProgress, 4, progress));

                    // 7. Sayım bilgisini JSON olarak kaydet
                    var parameters = new
                    {
                        SayimId = sayimId,
                        SayimKodu = sayimInfo.SayimKodu,
                        BrandId = sayimInfo.BrandID,
                        BrandName = sayimInfo.Brand?.BrandName,
                        StoreId = sayimInfo.StoreID,
                        StoreName = sayimInfo.Store?.StoreName,
                        ExportDate = DateTime.Now
                    };

                    string paramJson = JsonHelper.SerializeObject(parameters);
                    File.WriteAllText(Path.Combine(outputPath, "parameters.json"), paramJson);

                    MessageBox.Show($"Veri başarıyla SQLite veritabanına aktarıldı.\nDosya: {dbPath}",
                                   "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SQLite oluşturulurken hata: {ex.Message}",
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // İlerleme çubuğunu güncelleme
        private void UpdateProgress(int currentStep, int totalSteps, int stepProgress)
        {
            if (_progress != null)
            {
                int overallProgress = (int)((currentStep - 1) * 100.0 / totalSteps) + (int)(stepProgress * 100.0 / (totalSteps * 100.0));
                _progress.Report(Math.Min(overallProgress, 100));
            }
        }

        // Tüm tabloları oluşturma
        private void CreateTables(SQLiteConnection connection)
        {
            // CatalogItems tablosunu oluştur
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS CatalogItems (" +
                "Barcode TEXT PRIMARY KEY, " +
                "Description TEXT, " +
                "Category TEXT, " +
                "PrivateCode TEXT);", connection))
            {
                command.ExecuteNonQuery();
            }

            // SayimLokasyon tablosunu oluştur
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS SayimLokasyon (" +
                "Id INTEGER PRIMARY KEY, " +
                "Adi TEXT, " +
                "AlanKod TEXT, " +
                "Aciklama TEXT, " +
                "Miktar INTEGER, " +
                "SayimId INTEGER, " +
                "Aktif INTEGER, " +
                "IptalAciklama TEXT);", connection))
            {
                command.ExecuteNonQuery();
            }

            // SayimLokasyonDetay tablosunu oluştur
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS SayimLokasyonDetay (" +
                "Id INTEGER PRIMARY KEY, " +
                "SayimId INTEGER, " +
                "AlanKod TEXT, " +
                "LokasyonKod TEXT, " +
                "SayimLokasyonId INTEGER, " +
                "Aktif INTEGER, " +
                "IptalAciklama TEXT);", connection))
            {
                command.ExecuteNonQuery();
            }

            // SayimPersonel tablosunu oluştur
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS SayimPersonel (" +
                "Id INTEGER PRIMARY KEY, " +
                "SayimId INTEGER, " +
                "TcNo TEXT, " +
                "Adi TEXT, " +
                "Soyadi TEXT, " +
                "Tip INTEGER);", connection))
            {
                command.ExecuteNonQuery();
            }

            // Users tablosunu oluştur (JavaScript örneği için)
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS users (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name TEXT, " +
                "email TEXT, " +
                "age INTEGER, " +
                "is_active INTEGER);", connection))
            {
                command.ExecuteNonQuery();
            }

            // Products tablosunu oluştur (JavaScript örneği için)
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS products (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name TEXT, " +
                "category TEXT, " +
                "price REAL, " +
                "stock_count INTEGER);", connection))
            {
                command.ExecuteNonQuery();
            }

            // İndeksler oluştur
            using (var command = new SQLiteCommand(
                "CREATE INDEX IF NOT EXISTS IDX_SayimLokasyon_SayimId ON SayimLokasyon(SayimId);", connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                "CREATE INDEX IF NOT EXISTS IDX_SayimLokasyonDetay_SayimId ON SayimLokasyonDetay(SayimId);", connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                "CREATE INDEX IF NOT EXISTS IDX_SayimLokasyonDetay_SayimLokasyonId ON SayimLokasyonDetay(SayimLokasyonId);", connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                "CREATE INDEX IF NOT EXISTS IDX_SayimPersonel_SayimId ON SayimPersonel(SayimId);", connection))
            {
                command.ExecuteNonQuery();
            }

            // JavaScript için test verileri ekle
            InsertTestDataForJavaScript(connection);
        }

        // JavaScript için test verileri ekleme
        private void InsertTestDataForJavaScript(SQLiteConnection connection)
        {
            try
            {
                // Users tablosu için test verileri
                string[] userInserts = {
                    "INSERT INTO users (name, email, age, is_active) VALUES ('Ahmet Yılmaz', 'ahmet@example.com', 28, 1)",
                    "INSERT INTO users (name, email, age, is_active) VALUES ('Ayşe Kaya', 'ayse@example.com', 34, 1)",
                    "INSERT INTO users (name, email, age, is_active) VALUES ('Mehmet Demir', 'mehmet@example.com', 45, 0)",
                    "INSERT INTO users (name, email, age, is_active) VALUES ('Zeynep Çelik', 'zeynep@example.com', 30, 1)",
                    "INSERT INTO users (name, email, age, is_active) VALUES ('Ali Öztürk', 'ali@example.com', 22, 1)"
                };

                // Products tablosu için test verileri
                string[] productInserts = {
                    "INSERT INTO products (name, category, price, stock_count) VALUES ('Laptop', 'Elektronik', 5999.99, 25)",
                    "INSERT INTO products (name, category, price, stock_count) VALUES ('Akıllı Telefon', 'Elektronik', 3499.50, 50)",
                    "INSERT INTO products (name, category, price, stock_count) VALUES ('Bluetooth Kulaklık', 'Aksesuar', 299.90, 100)",
                    "INSERT INTO products (name, category, price, stock_count) VALUES ('Masa Lambası', 'Ev Eşyası', 149.90, 75)",
                    "INSERT INTO products (name, category, price, stock_count) VALUES ('Spor Ayakkabı', 'Giyim', 899.90, 30)"
                };

                // Transaction başlat
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Users için verileri ekle
                        foreach (var insert in userInserts)
                        {
                            using (var command = new SQLiteCommand(insert, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        // Products için verileri ekle
                        foreach (var insert in productInserts)
                        {
                            using (var command = new SQLiteCommand(insert, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        // Transaction'ı commit et
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test verileri eklenirken hata: {ex.Message}");
                // Hata olsa bile işleme devam et
            }
        }

        // CatalogItems verisini SQL Server'dan alıp SQLite'a aktarma
        private void ImportCatalogItems(SQLiteConnection connection, int brandId, Action<int> progressCallback)
        {
            using (var sqlConnection = new SqlConnection(Global.ConnectionString))
            {
                sqlConnection.Open();

                // Önce toplam kayıt sayısını al
                string countQuery = "SELECT COUNT(*) FROM CatalogItems WHERE BrandID = @BrandID";
                using (var countCommand = new SqlCommand(countQuery, sqlConnection))
                {
                    countCommand.Parameters.AddWithValue("@BrandID", brandId);
                    int totalCount = (int)countCommand.ExecuteScalar();

                    if (totalCount == 0)
                    {
                        progressCallback?.Invoke(100); // No data to process
                        return;
                    }

                    // Verileri çek
                    string query = "SELECT Barcode, Description, Category, PrivateCode FROM CatalogItems WHERE BrandID = @BrandID";
                    using (var command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@BrandID", brandId);

                        // Batch işlem için okuyucu ve sayaçları hazırla
                        using (var reader = command.ExecuteReader())
                        {
                            int batchSize = 1000;
                            int processedCount = 0;
                            List<Dictionary<string, object>> batchData = new List<Dictionary<string, object>>();

                            // Tüm verileri oku
                            while (reader.Read())
                            {
                                // Geçerli satırın verilerini sözlüğe kaydet
                                var row = new Dictionary<string, object>
                                {
                                    { "Barcode", reader["Barcode"] },
                                    { "Description", reader["Description"] != DBNull.Value ? reader["Description"] : null },
                                    { "Category", reader["Category"] != DBNull.Value ? reader["Category"] : null },
                                    { "PrivateCode", reader["PrivateCode"] != DBNull.Value ? reader["PrivateCode"] : null }
                                };

                                batchData.Add(row);
                                processedCount++;

                                // Batch boyutuna ulaşıldığında veya son elemansa verileri yaz
                                if (batchData.Count >= batchSize || processedCount == totalCount)
                                {
                                    using (var transaction = connection.BeginTransaction())
                                    {
                                        try
                                        {
                                            foreach (var dataRow in batchData)
                                            {
                                                using (var insertCommand = new SQLiteCommand(
                                                    "INSERT INTO CatalogItems (Barcode, Description, Category, PrivateCode) " +
                                                    "VALUES (@Barcode, @Description, @Category, @PrivateCode)", connection))
                                                {
                                                    insertCommand.Parameters.AddWithValue("@Barcode", dataRow["Barcode"]);
                                                    insertCommand.Parameters.AddWithValue("@Description", dataRow["Description"] ?? DBNull.Value);
                                                    insertCommand.Parameters.AddWithValue("@Category", dataRow["Category"] ?? DBNull.Value);
                                                    insertCommand.Parameters.AddWithValue("@PrivateCode", dataRow["PrivateCode"] ?? DBNull.Value);

                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                            transaction.Commit();
                                        }
                                        catch
                                        {
                                            transaction.Rollback();
                                            throw;
                                        }
                                    }

                                    // Batch listesini temizle
                                    batchData.Clear();

                                    // İlerleme bildir
                                    progressCallback?.Invoke((int)((double)processedCount / totalCount * 100));
                                }
                            }
                        }
                    }
                }
            }
        }

        // SayimLokasyon verisini SQL Server'dan alıp SQLite'a aktarma
        private void ImportSayimLokasyon(SQLiteConnection connection, int sayimId, Action<int> progressCallback)
        {
            using (var sqlConnection = new SqlConnection(Global.ConnectionString))
            {
                sqlConnection.Open();

                // Verileri çek
                string query = "SELECT * FROM SayimLokasyon WHERE SayimId = @SayimId";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    using (var reader = command.ExecuteReader())
                    {
                        int count = 0;
                        int totalCount = 0;

                        // Kayıt sayısını al
                        var lokasyonlar = Global.SayimLokasyonRepository.Find(l => l.SayimId == sayimId);
                        totalCount = lokasyonlar.Count();

                        if (totalCount == 0)
                        {
                            progressCallback?.Invoke(100); // No data to process
                            return;
                        }

                        // SQLite transaction başlat
                        var transaction = connection.BeginTransaction();

                        try
                        {
                            while (reader.Read())
                            {
                                using (var insertCommand = new SQLiteCommand(
                                    "INSERT INTO SayimLokasyon (Id, Adi, AlanKod, Aciklama, Miktar, SayimId, Aktif, IptalAciklama) " +
                                    "VALUES (@Id, @Adi, @AlanKod, @Aciklama, @Miktar, @SayimId, @Aktif, @IptalAciklama)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", reader["Id"]);
                                    insertCommand.Parameters.AddWithValue("@Adi", reader["Adi"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@AlanKod", reader["AlanKod"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@Aciklama", reader["Aciklama"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@Miktar", reader["Miktar"]);
                                    insertCommand.Parameters.AddWithValue("@SayimId", reader["SayimId"]);
                                    insertCommand.Parameters.AddWithValue("@Aktif", reader["Aktif"]);
                                    insertCommand.Parameters.AddWithValue("@IptalAciklama", reader["IptalAciklama"] ?? DBNull.Value);

                                    insertCommand.ExecuteNonQuery();
                                    count++;

                                    // İlerleme bildir (her 10 kayıtta bir)
                                    if (count % 10 == 0)
                                        progressCallback?.Invoke((int)((double)count / totalCount * 100));
                                }
                            }

                            transaction.Commit();
                            progressCallback?.Invoke(100); // Tamamlandı
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        // SayimLokasyonDetay verisini SQL Server'dan alıp SQLite'a aktarma
        private void ImportSayimLokasyonDetay(SQLiteConnection connection, int sayimId, Action<int> progressCallback)
        {
            using (var sqlConnection = new SqlConnection(Global.ConnectionString))
            {
                sqlConnection.Open();

                // Verileri çek
                string query = "SELECT * FROM SayimLokasyonDetay WHERE SayimId = @SayimId";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    using (var reader = command.ExecuteReader())
                    {
                        int count = 0;
                        int totalCount = 0;

                        // Kayıt sayısını al
                        var detaylar = Global.SayimLokasyonDetayRepository.Find(d => d.SayimId == sayimId);
                        totalCount = detaylar.Count();

                        if (totalCount == 0)
                        {
                            progressCallback?.Invoke(100); // No data to process
                            return;
                        }

                        // SQLite transaction başlat
                        var transaction = connection.BeginTransaction();

                        try
                        {
                            while (reader.Read())
                            {
                                using (var insertCommand = new SQLiteCommand(
                                    "INSERT INTO SayimLokasyonDetay (Id, SayimId, AlanKod, LokasyonKod, SayimLokasyonId, Aktif, IptalAciklama) " +
                                    "VALUES (@Id, @SayimId, @AlanKod, @LokasyonKod, @SayimLokasyonId, @Aktif, @IptalAciklama)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", reader["Id"]);
                                    insertCommand.Parameters.AddWithValue("@SayimId", reader["SayimId"]);
                                    insertCommand.Parameters.AddWithValue("@AlanKod", reader["AlanKod"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@LokasyonKod", reader["LokasyonKod"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@SayimLokasyonId", reader["SayimLokasyonId"]);
                                    insertCommand.Parameters.AddWithValue("@Aktif", reader["Aktif"]);
                                    insertCommand.Parameters.AddWithValue("@IptalAciklama", reader["IptalAciklama"] ?? DBNull.Value);

                                    insertCommand.ExecuteNonQuery();
                                    count++;

                                    // İlerleme bildir (her 10 kayıtta bir)
                                    if (count % 10 == 0)
                                        progressCallback?.Invoke((int)((double)count / totalCount * 100));
                                }
                            }

                            transaction.Commit();
                            progressCallback?.Invoke(100); // Tamamlandı
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        // SayimPersonel verisini SQL Server'dan alıp SQLite'a aktarma
        private void ImportSayimPersonel(SQLiteConnection connection, int sayimId, Action<int> progressCallback)
        {
            using (var sqlConnection = new SqlConnection(Global.ConnectionString))
            {
                sqlConnection.Open();

                // Verileri çek
                string query = "SELECT * FROM SayimPersonel WHERE SayimId = @SayimId";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@SayimId", sayimId);

                    using (var reader = command.ExecuteReader())
                    {
                        int count = 0;
                        int totalCount = 0;

                        // Kayıt sayısını al
                        var personeller = Global.SayimPersonelRepository.Find(d => d.SayimId == sayimId);
                        totalCount = personeller?.Count() ?? 0;

                        if (totalCount == 0)
                        {
                            progressCallback?.Invoke(100); // No data to process
                            return;
                        }

                        // SQLite transaction başlat
                        var transaction = connection.BeginTransaction();

                        try
                        {
                            while (reader.Read())
                            {
                                using (var insertCommand = new SQLiteCommand(
                                    "INSERT INTO SayimPersonel (Id, SayimId, TcNo, Adi, Soyadi, Tip) " +
                                    "VALUES (@Id, @SayimId, @TcNo, @Adi, @Soyadi, @Tip)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", reader["Id"]);
                                    insertCommand.Parameters.AddWithValue("@SayimId", reader["SayimId"]);
                                    insertCommand.Parameters.AddWithValue("@TcNo", reader["TcNo"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@Adi", reader["Adi"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@Soyadi", reader["Soyadi"] ?? DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@Tip", reader["Tip"]);

                                    insertCommand.ExecuteNonQuery();
                                    count++;

                                    // İlerleme bildir (her 5 kayıtta bir)
                                    if (count % 5 == 0)
                                        progressCallback?.Invoke((int)((double)count / totalCount * 100));
                                }
                            }

                            transaction.Commit();
                            progressCallback?.Invoke(100); // Tamamlandı
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
    }
}