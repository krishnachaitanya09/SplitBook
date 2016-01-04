using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Currency
    {
        [Unique]
        public string currency_code { get; set; }
        public string unit { get; set; }

        public override string ToString()
        {
            return currency_code;
        }
    }
}
