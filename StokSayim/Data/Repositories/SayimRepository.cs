using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StokSayim.Models;
using System.Data.Entity;


namespace StokSayim.Data.Repositories
{
    public class SayimRepository : BaseRepository<Models.Sayim>
    {
        public SayimRepository(DatabaseContext context) : base(context)
        {
        }

       // İlişkili verileri dahil ederek tüm sayımları getir
        public List<Models.Sayim> GetAllWithRelations()
        {
            try
            {
                // Context'in varlığını kontrol et
                if (_context == null)
                {
                    throw new InvalidOperationException("Database context is null");
                }

                // İlişkili tabloları dahil ederek veri çek
                var result = _context.Set<Models.Sayim>()
                    .Include(s => s.Brand)
                    .Include(s => s.Store)
                    .OrderByDescending(s => s.OlusturmaTarihi)
                    .ToList();

                Console.WriteLine($"GetAllWithRelations: {result.Count} sayım kaydı bulundu.");
                return result;
            }
            catch (Exception ex)
            {
                // Hata durumunda detaylı loglama
                Console.WriteLine($"GetAllWithRelations hatası: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        // Sayım ID'sine göre ilişkili verilerle birlikte getir
        public Models.Sayim GetByIdWithRelations(int id)
        {
            return _context.Set<Models.Sayim>()
                .Include(s => s.Brand)
                .Include(s => s.Store)
                .FirstOrDefault(s => s.Id == id);
        }

        // Mağaza ID'sine göre sayımları getir
        public List<Models.Sayim> GetByStoreId(int storeId)
        {
            return _context.Set<Models.Sayim>()
                .Include(s => s.Brand)
                .Include(s => s.Store)
                .Where(s => s.StoreID == storeId)
                .OrderByDescending(s => s.OlusturmaTarihi)
                .ToList();
        }

        // Marka ID'sine göre sayımları getir
        public List<Models.Sayim> GetByBrandId(int brandId)
        {
            return _context.Set<Models.Sayim>()
                .Include(s => s.Brand)
                .Include(s => s.Store)
                .Where(s => s.BrandID == brandId)
                .OrderByDescending(s => s.OlusturmaTarihi)
                .ToList();
        }

        // Duruma göre sayımları getir (0: Başladı, 1: Tamamlandı, 2: İptal Edildi)
        public List<Models.Sayim> GetByStatus(int status)
        {
            return _context.Set<Models.Sayim>()
                .Include(s => s.Brand)
                .Include(s => s.Store)
                .Where(s => s.SayimDurumu == status)
                .OrderByDescending(s => s.OlusturmaTarihi)
                .ToList();
        }

        // Direkt olarak yeni context oluşturarak ilişkili verileri getir (bağlantı sorunları için)
        public List<Models.Sayim> GetAllWithRelationsDirect()
        {
            // Yeni bir context oluştur
            using (var newContext = new DatabaseContext(Global.ConnectionString))
            {
                var result = newContext.Set<Models.Sayim>()
                    .Include(s => s.Brand)
                    .Include(s => s.Store)
                    .OrderByDescending(s => s.OlusturmaTarihi)
                    .ToList();

                return result;
            }
        }
    }
}

