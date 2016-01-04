using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Debt_Expense
    {
        public int from { get; set; }
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        public int expense_id { get; set; }
        [Ignore]
        public User fromUser { get; set; }
        [Ignore]
        public User toUser { get; set; }
    }
}
