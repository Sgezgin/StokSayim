using StokSayim.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Helpers
{
    public static class TextFileHelper
    {
        public static DataTable ReadTextFile(string filePath, out string errorMessage, string delimiter = null)
        {
            errorMessage = null;
            try
            {
                // Dosyayı oku
                var lines = File.ReadAllLines(filePath, Encoding.GetEncoding("windows-1254"));

                // Boş dosya kontrolü
                if (lines.Length == 0)
                {
                    errorMessage = "Dosya boş.";
                    return null;
                }

                // Ayırıcı belirlenmemişse, otomatik tespit et
                if (string.IsNullOrEmpty(delimiter))
                {
                    delimiter = DetectDelimiter(lines[0]);
                }

                // DataTable oluştur
                DataTable dataTable = new DataTable();

                // Başlık satırını ayır
                string[] headers = lines[0].Split(delimiter[0]);

                // Başlık satırında boş sütun adlarını düzelt
                for (int i = 0; i < headers.Length; i++)
                {
                    headers[i] = string.IsNullOrWhiteSpace(headers[i]) ? $"Column{i + 1}" : headers[i].Trim();

                    // Tekrarlanan başlık adlarını düzelt
                    string headerName = headers[i];
                    int counter = 1;

                    while (dataTable.Columns.Contains(headerName))
                    {
                        headerName = $"{headers[i]}_{counter++}";
                    }

                    dataTable.Columns.Add(headerName);
                }

                // Veri satırlarını ekle
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    // Boş satırları atla
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // Satırı ayırıcı karaktere göre böl
                    string[] values = line.Split(delimiter[0]);

                    // Yeni satır oluştur
                    DataRow dataRow = dataTable.NewRow();

                    // Değerleri ekle
                    for (int j = 0; j < Math.Min(values.Length, headers.Length); j++)
                    {
                        dataRow[j] = values[j].Trim();
                    }

                    dataTable.Rows.Add(dataRow);
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                errorMessage = $"Metin dosyası okunurken hata oluştu: {ex.Message}";
                return null;
            }
        }

        public static string DetectDelimiter(string line)
        {
            // Yaygın ayırıcılar
            string[] commonDelimiters = { ";", ",", "\t", "|" };

            // En çok kullanılan ayırıcıyı bul
            return commonDelimiters.OrderByDescending(d => line.Count(c => c == d[0])).First();
        }

        public static List<ColumnMapping> GenerateColumnMappings(DataTable dataTable)
        {
            // Excel Helper ile aynı mantık
            return ExcelHelper.GenerateColumnMappings(dataTable);
        }
    }
}
