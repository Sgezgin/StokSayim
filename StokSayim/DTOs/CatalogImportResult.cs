using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.DTOs
{
    public class CatalogImportResult
    {
        public bool Success { get; set; }
        public int ProcessedRows { get; set; }
        public int TotalCount { get; set; }
        public int ImportedRows { get; set; }
        public int FailedRows { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public TimeSpan ElapsedTime { get; set; }
    }
}
