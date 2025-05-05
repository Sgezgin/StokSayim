using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Repositories
{
    public class SayimPersonelRepository : BaseRepository<SayimPersonel>
    {
        public SayimPersonelRepository(DatabaseContext context) : base(context)
        {
        }


    }
}
