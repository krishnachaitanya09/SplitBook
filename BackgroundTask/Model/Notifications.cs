using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackgroundTasks.Model
{
    public sealed class Notifications
    {
        public int id { get; set; }
        public int type { get; set; }

        private string _created_at;
        public string created_at
        {
            get { return _created_at; }
            set
            {
                _created_at = DateTime.Parse(value).ToString("dd MMMM hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        public int created_by { get; set; }
        public Source source { get; set; }
        public string image_url { get; set; }

        private string _content;
        public string content
        {
            get { return _content; }
            set
            {
                value = value.Replace("<br>", "\n");
                _content = Regex.Replace(value, "<.*?>", String.Empty);
            }
        }
    }

    public sealed class Source
    {
        public string type { get; set; }
        public int id { get; set; }
        public object url { get; set; }
    }
}
