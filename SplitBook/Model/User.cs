using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class User
    {
        public static string DEFAULT_PROFILE_IMAGE_URL = @"https://dx0qysuen8cbs.cloudfront.net/assets/fat_rabbit/avatars/100-5eb999e2b4b24b823a9d82c29d42e9b2.png";

        [Unique]
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        [Ignore]
        public Picture picture { get; set; }
        
        public string email { get; set; }
        public string country_code { get; set; }
        public string default_currency { get; set; }

        [Ignore]
        public List<Balance_User> balance { get; set; }

        [Ignore]
        public List<Group> groups { get; set; }


        public string updated_at { get; set; }

        [Ignore]
        public string name
        {
            get
            {
                if (!String.IsNullOrEmpty(last_name))
                {
                    //char last = last_name[0];
                    //return first_name + " " + last + ".";
                    return first_name + " " + last_name;
                }
                else
                    return first_name;
            }
        }

        [Ignore]
        public string PictureUrl
        {
            get
            {
                if (picture!=null)
                {
                    return picture.medium;
                }
                else
                    return DEFAULT_PROFILE_IMAGE_URL;
            }
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            User p = obj as User;
            if ((System.Object)p == null)
            {
                return false;
            }

            return p.id == id;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
