using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Debt_Expense
    {
        [Key]
        public int id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }        
        public int expense_id { get; set; }
        [ForeignKey("expense_id")]
        public Debt_Expense debt_Expense { get; set; }
        [ForeignKey("from")]
        public User fromUser { get; set; }
        [ForeignKey("to")]
        public User toUser { get; set; }
    }
}
