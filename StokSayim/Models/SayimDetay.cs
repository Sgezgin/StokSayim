using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class SayimDetay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SayimID { get; set; }

        [Required]
        [MaxLength(15)]
        public string Barkod { get; set; }

        [Column(TypeName = "float")]
        public float SayimMiktari { get; set; } = 0;

        [Column(TypeName = "float")]
        public float DuzeltmeMiktar { get; set; } = 0;

        [Required]
        public DateTime SayimTarihi { get; set; }

        [MaxLength(15)]
        public string PersonelID { get; set; }

        public DateTime? EklemeTarih { get; set; }

        public DateTime? DuzenlemeTarih { get; set; }

        public int? TransferId { get; set; }

        public long? LokasyonId { get; set; }

        [ForeignKey("SayimID")]
        public virtual Sayim Sayim { get; set; }


        [ForeignKey("PersonelID")]
        public virtual Personel SayanPersonel { get; set; }
    }
}
