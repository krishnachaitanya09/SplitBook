using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Currency
    {
        public string currency_code { get; set; }
        public string unit { get; set; }

        public override string ToString()
        {
            return currency_code;
        }
    }
}
