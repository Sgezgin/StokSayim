using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Repositories
{
    public class SayimRepository : BaseRepository<Models.Sayim>
    {
        public SayimRepository(DatabaseContext context) : base(context)
        {
        }

        // Diğer özel  metodlar...
    }
}
