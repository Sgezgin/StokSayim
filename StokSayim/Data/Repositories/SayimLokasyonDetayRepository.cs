using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StokSayim.Models;

namespace StokSayim.Data.Repositories
{
    public class SayimLokasyonDetayRepository : BaseRepository<SayimLokasyonDetay>
    {
        public SayimLokasyonDetayRepository(DatabaseContext context) : base(context)
        {
        }

        
    }
}
