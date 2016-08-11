using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Debt_Group
    {
        [Key]
        public int from { get; set; }
        [Key]
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        [Key]
        public int group_id { get; set; }

        [ForeignKey("group_id")]
        public Group group { get; set; }

        [ForeignKey("from")]
        public User fromUser { get; set; }
        [ForeignKey("to")]
        public User toUser { get; set; }

        //the following is used in group summary expandable list
        [NotMapped]
        public int ownerId { get; set; }

        //need a copy contructor for this
        public Debt_Group(Debt_Group other)
        {
            this.from = other.from;
            this.to = other.to;
            this.currency_code = other.currency_code;
            this.amount = other.amount;
            this.group_id = other.group_id;
            this.fromUser = other.fromUser;
            this.toUser = other.toUser;
            this.ownerId = other.ownerId;
        }

        public Debt_Group()
        {
        }
    }
}
