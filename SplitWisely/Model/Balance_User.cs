using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class Balance_User
    {
        public string currency_code { get; set; }
        public string amount { get; set; }
        [Key]
        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User User { get; set; }
    }
}
