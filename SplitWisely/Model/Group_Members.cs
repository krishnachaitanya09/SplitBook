using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    class Group_Members
    {
        [Key]
        public int id { get; set; }
        public int group_id { get; set; }
        [ForeignKey("group_id")]
        public Group group { get; set; }
        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User user { get; set; }
    }
}
