using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Data.Repositories
{
    public class PersonelRepository : BaseRepository<Personel>
    {
        public PersonelRepository(DatabaseContext context) : base(context)
        {
        }

        // Diğer özel store metodları...
    }
}
