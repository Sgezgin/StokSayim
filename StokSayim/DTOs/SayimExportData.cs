using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.DTOs
{
    public class SayimExportData
    {
        public int SayimId { get; set; }
        public int BrandId { get; set; }
        public DateTime ExportDate { get; set; }
        public List<CatalogDto> Catalog { get; set; }
        public List<LocationDto> Locations { get; set; }
        public List<PersonelDto> Personels { get; set; }
    }

    public class CatalogDto
    {
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }

    public class LocationDto
    {
        public long Id { get; set; }
        public string Adi { get; set; }
        public string AlanKod { get; set; }
    }

    public class PersonelDto
    {
        public long Id { get; set; }
        public string TcNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
    }
}
