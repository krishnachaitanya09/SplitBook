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
    public class Comment
    {
        [Unique]
        public int id { get; set; }
        public string content { get; set; }
        public string comment_type { get; set; }
        public string relation_type { get; set; }
        public int relation_id { get; set; }
        public string created_at { get; set; }
        public string deleted_at { get; set; }
        [ForeignKey(typeof(User))]
        public int user_id { get; set; }
        [OneToOne("user_id", CascadeOperations = CascadeOperation.CascadeRead)]
        public User user { get; set; }
    }
}
