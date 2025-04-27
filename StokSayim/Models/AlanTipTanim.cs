using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class AlanTipTanim
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Adi { get; set; }

        [MaxLength(10)]
        public string AlanKod { get; set; }

        [MaxLength(150)]
        public string Aciklama { get; set; }

        public int Aktif { get; set; }

    }
}
