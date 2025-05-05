using Newtonsoft.Json;
using StokSayim.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Services
{
    public class BulkImportService
    {
        private readonly string _connectionString;

        public BulkImportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CatalogImportResult> BulkImportCatalogAsync(
            DataTable sourceData,
            int brandId,
            List<ColumnMapping> columnMappings,
            IProgress<int> progress = null)
        {
            var result = new CatalogImportResult();
            var startTime = DateTime.Now;

            try
            {
                // Hedef DataTable oluştur
                DataTable catalogData = new DataTable();
                catalogData.Columns.Add("Barcode", typeof(string));
                catalogData.Columns.Add("Description", typeof(string));
                catalogData.Columns.Add("Category", typeof(string));
                catalogData.Columns.Add("BrandID", typeof(int));
                catalogData.Columns.Add("CustomFields", typeof(string));
                catalogData.Columns.Add("CreatedDate", typeof(DateTime));
                catalogData.Columns.Add("UpdatedDate", typeof(DateTime));
                catalogData.Columns.Add("PrivateCode", typeof(string));


                // Standart alan eşleştirmeleri
                var barcodeMapping = columnMappings.FirstOrDefault(m => m.DestinationField == "Barcode");
                var descriptionMapping = columnMappings.FirstOrDefault(m => m.DestinationField == "Description");
                var categoryMapping = columnMappings.FirstOrDefault(m => m.DestinationField == "Category");

                var privateCodeMapping = columnMappings.FirstOrDefault(m =>
           m.DestinationField == "PrivateCode" ||
           m.DestinationField == "Özel Kod" ||
           m.DestinationField == "OzelKod");

                // Barkod mapping kontrolü
                if (barcodeMapping == null || string.IsNullOrEmpty(barcodeMapping.SourceColumn))
                {
                    throw new Exception("Barkod alanı için bir eşleştirme yapılmalıdır.");
                }

                // Toplam ve işlenen satır sayacı
                int totalRows = sourceData.Rows.Count;

                result.TotalCount = totalRows;
                int processedRows = 0;
                int batchSize = 1000; // Toplu işlem boyutu

                // Özel alan haritası (JSON oluşturmak için)
                var customFieldMappings = columnMappings
                    .Where(m => m.IsCustomField)
                    .ToDictionary(m => m.DestinationField, m => m.SourceColumn);

                // Kaynak verilerden hedef DataTable'a dönüşüm
                foreach (DataRow sourceRow in sourceData.Rows)
                {
                    // Barkod kontrolü (boşsa atla)
                    string barcode = GetSourceColumnValue(sourceRow, barcodeMapping.SourceColumn);
                    if (string.IsNullOrWhiteSpace(barcode))
                    {
                        result.FailedRows++;
                        continue;
                    }

                    // Özel alanları JSON olarak hazırla
                    var customFields = new Dictionary<string, object>();
                    foreach (var mapping in customFieldMappings)
                    {
                        string fieldName = mapping.Key;
                        string columnName = mapping.Value;

                        if (!string.IsNullOrEmpty(columnName))
                        {
                            string value = GetSourceColumnValue(sourceRow, columnName);
                            if (!string.IsNullOrEmpty(value))
                            {
                                customFields[fieldName] = value;
                            }
                        }
                    }

                    // Yeni satır oluştur
                    DataRow newRow = catalogData.NewRow();
                    newRow["Barcode"] = barcode;
                    newRow["Description"] = GetSourceColumnValue(sourceRow, descriptionMapping?.SourceColumn);
                    newRow["Category"] = GetSourceColumnValue(sourceRow, categoryMapping?.SourceColumn);
                    newRow["BrandID"] = brandId;
                    newRow["CustomFields"] = JsonConvert.SerializeObject(customFields);
                    newRow["CreatedDate"] = DateTime.Now;
                    newRow["UpdatedDate"] = DateTime.Now;

                    newRow["PrivateCode"] = privateCodeMapping != null ?
            GetSourceColumnValue(sourceRow, privateCodeMapping.SourceColumn) :
            string.Empty;

                    catalogData.Rows.Add(newRow);
                    processedRows++;

                    // Batch işlem kontrolü
                    if (catalogData.Rows.Count >= batchSize)
                    {
                        await BulkInsertToDbAsync(catalogData);
                        result.ImportedRows += catalogData.Rows.Count;
                        catalogData.Clear();

                        // İlerleme raporu
                        progress?.Report((int)((double)processedRows / totalRows * 100));
                    }
                }

                // Kalan satırları işle
                if (catalogData.Rows.Count > 0)
                {
                    await BulkInsertToDbAsync(catalogData);
                    result.ImportedRows += catalogData.Rows.Count;
                }

                result.Success = true;
                result.ProcessedRows = processedRows;
                result.ElapsedTime = DateTime.Now - startTime;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.ElapsedTime = DateTime.Now - startTime;
                return result;
            }
        }

        private async Task BulkInsertToDbAsync(DataTable dataTable)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "CatalogItems";
                            bulkCopy.BatchSize = dataTable.Rows.Count;

                            // Sütun eşleştirmelerini ayarla
                            bulkCopy.ColumnMappings.Add("Barcode", "Barcode");
                            bulkCopy.ColumnMappings.Add("Description", "Description");
                            bulkCopy.ColumnMappings.Add("Category", "Category");
                            bulkCopy.ColumnMappings.Add("BrandID", "BrandID");
                            bulkCopy.ColumnMappings.Add("CustomFields", "CustomFields");
                            bulkCopy.ColumnMappings.Add("CreatedDate", "CreatedDate");
                            bulkCopy.ColumnMappings.Add("UpdatedDate", "UpdatedDate");
                            bulkCopy.ColumnMappings.Add("PrivateCode", "PrivateCode");


                            await bulkCopy.WriteToServerAsync(dataTable);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private string GetSourceColumnValue(DataRow row, string columnName)
        {
            if (string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            return row[columnName]?.ToString() ?? string.Empty;
        }


    }
}
