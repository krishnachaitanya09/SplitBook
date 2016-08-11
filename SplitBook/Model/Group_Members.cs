using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Group_Members
    {
        [Key]
        public int group_id { get; set; }
        [Key]
        public int user_id { get; set; }
        [ForeignKey("group_id")]
        public Group group { get; set; }
    }
}
