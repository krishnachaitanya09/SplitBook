using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Group
    {
        [Unique]
        public int id { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public bool simplify_by_default { get; set; }

        [Ignore]
        public List<User> members { get; set; }
        [Ignore]
        public List<Group_Members> group_members { get; set; }
        [Ignore]
        public List<Debt_Group> original_debts { get; set; }

        [Ignore]
        public List<Debt_Group> simplified_debts { get; set; }
        
        public string whiteboard { get; set; }
        public string group_type { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
