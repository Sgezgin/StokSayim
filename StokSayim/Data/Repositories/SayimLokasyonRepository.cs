using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StokSayim.Models;

namespace StokSayim.Data.Repositories
{
    public class SayimLokasyonRepository : BaseRepository<SayimLokasyon>
    {
        public SayimLokasyonRepository(DatabaseContext context) : base(context)
        {
        }

        // Diğer özel brand metodları...
    }
}
