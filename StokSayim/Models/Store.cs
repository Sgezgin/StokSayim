using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokSayim.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }

        [MaxLength(50)]
        public string StoreCode { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string StoreName { get; set; }

        public int BrandID { get; set; }

        public int? IlId { get; set; }

        [MaxLength(50)]
        public string IlceAdi { get; set; }

        [MaxLength(250)]
        public string Adres { get; set; } = "";

        [MaxLength(50)]
        public string Telefon { get; set; }

        [MaxLength(50)]
        public string Gsm { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string IlgiliKisi { get; set; } = "";

        [MaxLength(10)]
        public string OzelKod1 { get; set; } = "";

        [MaxLength(10)]
        public string OzelKod2 { get; set; } = "";

        [MaxLength(10)]
        public string OzelKod3 { get; set; } = "";

        public DateTime EklemeTarih { get; set; } = DateTime.Now;

        public DateTime? GuncellemeTarih { get; set; } = DateTime.Now;


        [ForeignKey("BrandID")]
        public virtual Brand Brand { get; set; }

        [ForeignKey("IlId")]
        public virtual IlTanim IlTanim { get; set; }
    }
}

