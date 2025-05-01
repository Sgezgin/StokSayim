using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class SayimPersonel
    {
        [Key]
        [Required]
        public long Id { get; set; }

        public int SayimId { get; set; }

        [MaxLength(12)]
        public string TcNo { get; set; }

        [MaxLength(50)]
        public string Adi { get; set; }

        [MaxLength(50)]
        public string Soyadi { get; set; }

        [Required]
        [Range(0, 9, ErrorMessage = "Tip sadece 0 ile 9 arasında bir değer alabilir.")]
        public int Tip { get; set; }

        [NotMapped]
        public string AdSoyad => $"{Adi} {Soyadi}";
    }
}
