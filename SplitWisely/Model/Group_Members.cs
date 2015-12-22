using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    class Group_Members
    {
        [ForeignKey(typeof(Group))]
        public int group_id { get; set; }
        [ForeignKey(typeof(User))]
        public int user_id { get; set; }
    }
}
