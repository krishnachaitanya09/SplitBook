﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SplitBook.Model
{
    public class Group_Members
    {
        public int group_id { get; set; }
        public int user_id { get; set; }
    }
}
