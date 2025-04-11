using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class Personel
    {
        [Key]
        [Required]
        [MaxLength(12)]
        public string TcNo { get; set; }

        [MaxLength(50)]
        public string Adi { get; set; }

        [MaxLength(50)]
        public string Soyadi { get; set; }

        [Required]
        [Range(0, 9, ErrorMessage = "Tip sadece 0 ile 9 arasında bir değer alabilir.")]
        public int Tip { get; set; }

        public string Ekleyen { get; set; } = "";

        public string Duzenleyen { get; set; } = "";

        public DateTime? EklemeTarih { get; set; }

        public DateTime? DuzenlemeTarih { get; set; }
    }
}
