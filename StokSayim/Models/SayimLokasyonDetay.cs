using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StokSayim.Models
{
    public class SayimLokasyonDetay
    {
        [Key]
        public long Id { get; set; }

        public int SayimId { get; set; }

        [MaxLength(10)]
        public string AlanKod { get; set; }

        [MaxLength(20)]
        public string LokasyonKod { get; set; }

        public long SayimLokasyonId { get; set; }

        public int Aktif { get; set; }

        [MaxLength(250)]
        public string IptalAciklama { get; set; }

        [ForeignKey("SayimLokasyonId")]
        public virtual SayimLokasyon SayimLokasyon { get; set; }


    }
}
