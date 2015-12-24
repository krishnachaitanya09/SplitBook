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
    public class Group
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public bool simplify_by_default { get; set; }
        [NotMapped]
        public virtual List<User> members { get; set; }
        [NotMapped]    
        public virtual List<Debt_Group> original_debts { get; set; }
        [NotMapped]
        public virtual List<Debt_Group> simplified_debts { get; set; }
        
        public string whiteboard { get; set; }
        public string group_type { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
