using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class Picture
    {
        [Unique]
        public int user_id { get; set; }
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string original { get; set; }
    }
}
