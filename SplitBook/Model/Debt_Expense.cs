using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Debt_Expense
    {
        [Key]
        public int from { get; set; }
        [Key]
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        [Key]
        public int expense_id { get; set; }
        [ForeignKey("expense_id")]
        public Expense expense { get; set; }
        [NotMapped]
        public User fromUser { get; set; }
        [NotMapped]
        public User toUser { get; set; }
    }
}
