using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.DTOs
{
    public class ColumnMapping
    {
        public string DestinationField { get; set; }
        public string SourceColumn { get; set; }
        public bool IsCustomField { get; set; }
        public bool IsRequired { get; set; }
    }
}

