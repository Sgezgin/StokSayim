using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokSayim.Models
{
    public class SayimLokasyon
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(50)]
        public string Adi { get; set; }

        [MaxLength(10)]
        public string AlanKod { get; set; }

        [MaxLength(150)]
        public string Aciklama { get; set; }

        public int Miktar { get; set; }

        public int SayimId { get; set; }

        public int Aktif { get; set; }

        [MaxLength(250)]
        public string IptalAciklama { get; set; }

        public virtual ICollection<SayimLokasyonDetay> SayimLokasyonDetaylari { get; set; }

    }
}
