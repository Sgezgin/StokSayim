using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Repositories
{
    public class StoreRepository : BaseRepository<Store>
    {
        public StoreRepository(DatabaseContext context) : base(context)
        {
        }

        // Diğer özel store metodları...
    }
}
