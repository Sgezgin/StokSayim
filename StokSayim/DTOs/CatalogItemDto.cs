using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.DTOs
{
    public class CatalogItemDto
    {
        public long ItemID { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string CustomFields { get; set; }
        public Dictionary<string, object> CustomFieldsDictionary { get; set; }
    }
}
