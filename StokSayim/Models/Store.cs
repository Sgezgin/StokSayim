using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokSayim.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }

        [Required]
        [MaxLength(50)]
        public string StoreCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string StoreName { get; set; }

        public int BrandID { get; set; }

        [ForeignKey("BrandID")]
        public virtual Brand Brand { get; set; }
    }
}

