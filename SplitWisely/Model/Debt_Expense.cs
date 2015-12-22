using SQLite;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Debt_Expense
    {
        [ForeignKey(typeof(User))]
        public int from { get; set; }
        [ForeignKey(typeof(User))]
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        [ForeignKey(typeof(Expense))]
        public int expense_id { get; set; }
        [OneToOne("from")]
        public User fromUser { get; set; }
        [OneToOne("to")]
        public User toUser { get; set; }
    }
}
