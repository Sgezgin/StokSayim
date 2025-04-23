using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class Sayim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SayimKodu { get; set; }

        [Required]
        public DateTime BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }

        [Required]
        public int StoreID { get; set; }

        [Required]
        public int BrandID { get; set; }

        public int SayimDurumu { get; set; } = 0; // 0: Başladı, 1: Tamamlandı, 2: İptal Edildi

        [MaxLength(500)]
        public string Aciklama { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public DateTime? GuncellemeTarihi { get; set; }

        [ForeignKey("StoreID")]
        public virtual Store Store { get; set; }

        [ForeignKey("BrandID")]
        public virtual Brand Brand { get; set; }

 
    }
}
