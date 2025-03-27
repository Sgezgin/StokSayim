using StokSayim.Data.Repositories;
using StokSayim.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using StokSayim.Models;

namespace StokSayim.Data.Services
{
    public class CatalogService
    {
        private readonly CatalogRepository _catalogRepository;
        private readonly string _connectionString;

        public CatalogService(CatalogRepository catalogRepository, string connectionString)
        {
            _catalogRepository = catalogRepository;
            _connectionString = connectionString;
        }

        public async Task<CatalogItem> GetByBarcodeAsync(string barcode, int brandId)
        {
            return await _catalogRepository.GetByBarcodeAsync(barcode, brandId);
        }

        public async Task<CatalogItemDto> GetCatalogItemDetailsByBarcodeAsync(string barcode, int brandId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT c.ItemID, c.Barcode, c.Description, c.Category, c.BrandID, 
                           b.BrandName, c.CustomFields
                    FROM CatalogItems c
                    JOIN Brands b ON c.BrandID = b.BrandID
                    WHERE c.Barcode = @Barcode AND c.BrandID = @BrandID";

                var item = await connection.QueryFirstOrDefaultAsync<CatalogItemDto>(query, new
                {
                    Barcode = barcode,
                    BrandID = brandId
                });

                return item;
            }
        }

        public async Task<IEnumerable<CatalogItemDto>> SearchCatalogAsync(
            int brandId, string searchText, int pageNumber = 1, int pageSize = 100)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT c.ItemID, c.Barcode, c.Description, c.Category, c.BrandID, 
                           b.BrandName, c.CustomFields
                    FROM CatalogItems c
                    JOIN Brands b ON c.BrandID = b.BrandID
                    WHERE c.BrandID = @BrandID
                      AND (c.Barcode LIKE @Search 
                           OR c.Description LIKE @Search 
                           OR c.Category LIKE @Search)
                    ORDER BY c.Barcode
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                return await connection.QueryAsync<CatalogItemDto>(query, new
                {
                    BrandID = brandId,
                    Search = $"%{searchText}%",
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                });
            }
        }

        public async Task<int> DeleteCatalogByBrandAsync(int brandId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM CatalogItems WHERE BrandID = @BrandID";

                return await connection.ExecuteAsync(query, new { BrandID = brandId });
            }
        }

        public async Task<List<string>> GetUniqueCustomFieldKeysAsync(int brandId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // JSON_VALUE fonksiyonu aracılığıyla tüm JSON anahtarlarını topla
                // Not: Bu sorgu SQL Server 2016+ gerektirir
                string query = @"
                    SELECT DISTINCT j.[key]
                    FROM CatalogItems c
                    CROSS APPLY OPENJSON(c.CustomFields) AS j
                    WHERE c.BrandID = @BrandID
                    ORDER BY j.[key]";

                return (await connection.QueryAsync<string>(query, new { BrandID = brandId })).ToList();
            }
        }
    }
}
