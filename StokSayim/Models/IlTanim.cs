using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Models
{
    public class IlTanim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Adi { get; set; }
    }
}
