using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace StokSayim.Models
{
    public class CatalogItem
    {
        [Key]
        public long ItemID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; }

        public string Description { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        [MaxLength(100)]
        public string PrivateCode { get; set; }
        

        public int BrandID { get; set; }

        [ForeignKey("BrandID")]
        public virtual Brand Brand { get; set; }

        // JSON formatında özel alanlar
        public string CustomFields { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public Dictionary<string, object> CustomFieldsDictionary
        {
            get => string.IsNullOrEmpty(CustomFields) ? new Dictionary<string, object>() :
                  JsonConvert.DeserializeObject<Dictionary<string, object>>(CustomFields);
            set => CustomFields = JsonConvert.SerializeObject(value);
        }
    }
}
