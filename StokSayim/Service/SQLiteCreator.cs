using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Service
{
    public class SQLiteCreator
    {
        /// <summary>
        /// Temp klasörüne örnek bir SQLite veritabanı oluşturur ve yolunu döndürür
        /// </summary>
        /// <returns>Oluşturulan veritabanının tam dosya yolu</returns>
        public static string CreateDemoDatabase()
        {
            // Temp klasöründe veritabanı dosya yolu
            string tempFolder = Path.GetTempPath();
            string dbPath = Path.Combine(tempFolder, "deneme.db");

            Console.WriteLine($"Veritabanı oluşturuluyor: {dbPath}");

            // Eğer veritabanı zaten varsa sil
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Console.WriteLine("Mevcut veritabanı silindi.");
            }

            // SQLite bağlantısı oluştur
            SQLiteConnection.CreateFile(dbPath);

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                // Tabloları oluştur ve verileri ekle
                CreateTables(connection);
                InsertTestData(connection);

                connection.Close();
            }

            Console.WriteLine($"Veritabanı başarıyla oluşturuldu: {dbPath}");
            return dbPath;
        }

        /// <summary>
        /// Veritabanında tabloları oluşturur
        /// </summary>
        private static void CreateTables(SQLiteConnection connection)
        {
            // Users tablosu
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS users (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name TEXT, " +
                "email TEXT, " +
                "age INTEGER, " +
                "is_active INTEGER);", connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Users tablosu oluşturuldu.");
            }

            // Products tablosu
            using (var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS products (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name TEXT, " +
                "category TEXT, " +
                "price REAL, " +
                "stock_count INTEGER);", connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Products tablosu oluşturuldu.");
            }
        }

        /// <summary>
        /// Tablolara test verilerini ekler
        /// </summary>
        private static void InsertTestData(SQLiteConnection connection)
        {
            // Kullanıcı verileri
            var users = new[]
            {
                new { Name = "Ahmet Yılmaz", Email = "ahmet@example.com", Age = 28, IsActive = 1 },
                new { Name = "Ayşe Kaya", Email = "ayse@example.com", Age = 34, IsActive = 1 },
                new { Name = "Mehmet Demir", Email = "mehmet@example.com", Age = 45, IsActive = 0 },
                new { Name = "Zeynep Çelik", Email = "zeynep@example.com", Age = 30, IsActive = 1 },
                new { Name = "Ali Öztürk", Email = "ali@example.com", Age = 22, IsActive = 1 }
            };

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var user in users)
                {
                    using (var command = new SQLiteCommand(
                        "INSERT INTO users (name, email, age, is_active) VALUES (@name, @email, @age, @isActive)", connection))
                    {
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@age", user.Age);
                        command.Parameters.AddWithValue("@isActive", user.IsActive);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                Console.WriteLine($"{users.Length} kullanıcı eklendi.");
            }

            // Ürün verileri
            var products = new[]
            {
                new { Name = "Laptop", Category = "Elektronik", Price = 5999.99, StockCount = 25 },
                new { Name = "Akıllı Telefon", Category = "Elektronik", Price = 3499.50, StockCount = 50 },
                new { Name = "Bluetooth Kulaklık", Category = "Aksesuar", Price = 299.90, StockCount = 100 },
                new { Name = "Masa Lambası", Category = "Ev Eşyası", Price = 149.90, StockCount = 75 },
                new { Name = "Spor Ayakkabı", Category = "Giyim", Price = 899.90, StockCount = 30 }
            };

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var product in products)
                {
                    using (var command = new SQLiteCommand(
                        "INSERT INTO products (name, category, price, stock_count) VALUES (@name, @category, @price, @stockCount)", connection))
                    {
                        command.Parameters.AddWithValue("@name", product.Name);
                        command.Parameters.AddWithValue("@category", product.Category);
                        command.Parameters.AddWithValue("@price", product.Price);
                        command.Parameters.AddWithValue("@stockCount", product.StockCount);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                Console.WriteLine($"{products.Length} ürün eklendi.");
            }
        }

        
    }
}
