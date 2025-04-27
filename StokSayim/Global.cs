using StokSayim.Data;
using StokSayim.Data.Repositories;
using StokSayim.Data.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim
{
    /// <summary>
    /// Uygulama genelinde kullanılacak paylaşılan servisleri yönetir.
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Veritabanı bağlantı string'i
        /// </summary>
        public static string ConnectionString { get; private set; }

        /// <summary>
        /// Veritabanı context
        /// </summary>
        public static DatabaseContext DbContext { get; private set; }

        /// <summary>
        /// Marka repository
        /// </summary>
        public static BrandRepository BrandRepository { get; private set; }

        /// <summary>
        /// Mağaza repository
        /// </summary>
        public static StoreRepository StoreRepository { get; private set; }

        /// <summary>
        /// Katalog repository
        /// </summary>
        public static CatalogRepository CatalogRepository { get; private set; }

        /// <summary>
        /// Katalog servisi
        /// </summary>
        public static CatalogService CatalogService { get; private set; }

        /// <summary>
        /// Toplu içe aktarma servisi
        /// </summary>
        public static BulkImportService BulkImportService { get; private set; }

        public static PersonelRepository PersonelRepository { get; private set; }

        public static SayimRepository SayimRepository { get; private set; }
        public static SayimDetayRepository SayimDetayRepository { get; private set; }
        public static SayimLokasyonRepository SayimLokasyonRepository { get; private set; }
        public static SayimLokasyonDetayRepository SayimLokasyonDetayRepository { get; private set; }


        /// <summary>
        /// Global servisleri başlatır
        /// </summary>
        public static void Initialize()
        {
            // Bağlantı string'ini al
            ConnectionString = ConfigurationManager.ConnectionStrings["StokSayimConnection"].ConnectionString;
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ApplicationException("Veritabanı bağlantı bilgisi bulunamadı. App.config dosyasını kontrol edin.");
            }

            // Veritabanı context
            DbContext = new DatabaseContext(ConnectionString);

            // Repository'leri oluştur
            BrandRepository = new BrandRepository(DbContext);
            StoreRepository = new StoreRepository(DbContext);
            CatalogRepository = new CatalogRepository(DbContext);
            PersonelRepository = new PersonelRepository(DbContext);
            SayimRepository = new SayimRepository(DbContext);
            SayimDetayRepository = new SayimDetayRepository(DbContext);
            SayimLokasyonRepository = new SayimLokasyonRepository(DbContext);
            SayimLokasyonDetayRepository = new SayimLokasyonDetayRepository(DbContext);


            // Servisleri oluştur
            CatalogService = new CatalogService(CatalogRepository, ConnectionString);
            BulkImportService = new BulkImportService(ConnectionString);
        }
    }
}
