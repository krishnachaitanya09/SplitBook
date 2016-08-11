using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Group
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public bool simplify_by_default { get; set; }
        [NotMapped]
        public List<User> members { get; set; }
        public List<Group_Members> group_members { get; set; }
        [NotMapped]
        public List<Debt_Group> original_debts { get; set; }
        public List<Debt_Group> simplified_debts { get; set; }
        
        public string whiteboard { get; set; }
        public string group_type { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
