using StokSayim.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.IO;

namespace StokSayim.Helpers
{
    public static class ExcelHelper
    {
        static ExcelHelper()
        {
            // .NET Framework için Encoding provider'ı kaydet
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public static DataTable ReadExcelFile(string filePath, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Excel reader factory ile okuyucu oluştur
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Excel dosyasını DataSet olarak oku
                        var result = reader.AsDataSet(new ExcelDataReader.ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataReader.ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true // İlk satırı başlık olarak kullan
                            }
                        });

                        // İlk sayfayı al
                        if (result.Tables.Count > 0)
                        {
                            return result.Tables[0];
                        }
                        else
                        {
                            errorMessage = "Excel dosyasında veri bulunamadı.";
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Excel dosyası okunurken hata oluştu: {ex.Message}";
                return null;
            }
        }

        public static List<ColumnMapping> GenerateColumnMappings(DataTable dataTable)
        {
            var mappings = new List<ColumnMapping>();

            // Standart alanlar
            mappings.Add(new ColumnMapping
            {
                DestinationField = "Barcode",
                SourceColumn = FindBestMatchingColumn(dataTable, new[] { "Barcode", "Barkod", "BarcodeNo", "Barkod No" }),
                IsCustomField = false,
                IsRequired = true
            });

            mappings.Add(new ColumnMapping
            {
                DestinationField = "Description",
                SourceColumn = FindBestMatchingColumn(dataTable, new[] { "Description", "Açıklama", "Ürün Adı", "Ürün", "ProductName" }),
                IsCustomField = false,
                IsRequired = false
            });

            mappings.Add(new ColumnMapping
            {
                DestinationField = "Category",
                SourceColumn = FindBestMatchingColumn(dataTable, new[] { "Category", "Kategori", "ProductCategory" }),
                IsCustomField = false,
                IsRequired = false
            });

            // Diğer tüm sütunlar özel alan olarak eklenir
            foreach (DataColumn column in dataTable.Columns)
            {
                bool isStandardField = mappings.Any(m => m.SourceColumn == column.ColumnName);

                if (!isStandardField)
                {
                    mappings.Add(new ColumnMapping
                    {
                        DestinationField = column.ColumnName,
                        SourceColumn = column.ColumnName,
                        IsCustomField = true,
                        IsRequired = false
                    });
                }
            }

            return mappings;
        }

        private static string FindBestMatchingColumn(DataTable dataTable, string[] possibleColumnNames)
        {
            foreach (var columnName in possibleColumnNames)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    // Contains metodunu StringComparison ile kullanma düzeltmesi
                    if (column.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase) ||
                        column.ColumnName.IndexOf(columnName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return column.ColumnName;
                    }
                }
            }

            return null;
        }
    }
}
