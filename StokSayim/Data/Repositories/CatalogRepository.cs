﻿using Newtonsoft.Json.Linq;
using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Repositories
{
    public class CatalogRepository : BaseRepository<CatalogItem>
    {
        public CatalogRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<CatalogItem> GetByBarcodeAsync(string barcode, int brandId)
        {
            return await _context.CatalogItems
                .Where(c => c.Barcode == barcode && c.BrandID == brandId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CatalogItem>> GetByBrandAsync(int brandId, int pageNumber = 1, int pageSize = 100)
        {
            return await _context.CatalogItems
                .Where(c => c.BrandID == brandId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GetByBrand - senkron versiyon
        public IEnumerable<CatalogItem> GetByBrand(int brandId, int pageNumber = 1, int pageSize = 100)
        {
            return _context.CatalogItems
                .Where(c => c.BrandID == brandId)
                .OrderBy(c => c.Barcode) // Bir sıralama ekledim
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<int> GetCountByBrandAsync(int brandId)
        {
            return await _context.CatalogItems
                .Where(c => c.BrandID == brandId)
                .CountAsync();
        }

        // Özel alanların anahtarlarını getir
        public List<string> GetUniqueCustomFieldKeys(int brandId)
        {
            List<string> uniqueKeys = new List<string>();

            // Markaya ait katalog öğelerinden CustomFields değerleri al
            var items = _context.CatalogItems
                .Where(c => c.BrandID == brandId && !string.IsNullOrEmpty(c.CustomFields))
                .Select(c => c.CustomFields)
                .Take(100) // İlk 100 öğeyi analiz et (performans için)
                .ToList();

            // JSON alanlarından benzersiz anahtarları çıkart
            foreach (var jsonStr in items)
            {
                try
                {
                    if (!string.IsNullOrEmpty(jsonStr))
                    {
                        JObject jsonObj = JObject.Parse(jsonStr);
                        foreach (var property in jsonObj.Properties())
                        {
                            string key = property.Name;
                            if (!uniqueKeys.Contains(key))
                            {
                                uniqueKeys.Add(key);
                            }
                        }
                    }
                }
                catch
                {
                    // JSON ayrıştırma hatalarını atla
                    continue;
                }
            }

            return uniqueKeys;
        }
    }
}
