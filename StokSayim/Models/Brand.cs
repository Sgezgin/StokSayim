using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StokSayim.Models
{
    public class Brand
    {
        [Key]
        public int BrandID { get; set; }

        [Required]
        [MaxLength(50)]
        public string BrandCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string BrandName { get; set; }

        // Navigation properties
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<CatalogItem> CatalogItems { get; set; }
    }
}
