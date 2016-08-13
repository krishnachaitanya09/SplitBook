using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Receipt
    {
        [Key]
        public int expense_id { get; set; }
        public string large { get; set; }
        public string original { get; set; }
        [ForeignKey("expense_id")]
        public Expense Expense { get; set; }
    }
}
