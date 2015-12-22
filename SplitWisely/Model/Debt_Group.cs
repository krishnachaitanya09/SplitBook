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
    public class Debt_Group
    {
        [ForeignKey(typeof(User))]
        public int from { get; set; }
        [ForeignKey(typeof(User))]
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        [ForeignKey(typeof(Group))]
        public int group_id { get; set; }

        [OneToOne("from")]
        public User fromUser { get; set; }
        [OneToOne("to")]
        public User toUser { get; set; }

        //the following is used in group summary expandable list
        [Ignore]
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
