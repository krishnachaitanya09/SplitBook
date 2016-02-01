using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitBook.Converter
{
    public class DateStringToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string created_at = value as string;
            DateTime createdDate = DateTime.Parse(created_at, System.Globalization.CultureInfo.InvariantCulture);
            string dateString = createdDate.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            return dateString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
