using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Comment
    {
        [Key]
        public int id { get; set; }
        public string content { get; set; }
        public string comment_type { get; set; }
        public string relation_type { get; set; }
        public int relation_id { get; set; }
        public string created_at { get; set; }
        public string deleted_at { get; set; }        
        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User user { get; set; }
    }
}
