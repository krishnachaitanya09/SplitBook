using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SplitWisely.Converter.UserDetails 
{
    public class UserDetailsExpenseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush colorBrush;
            double finalBalance = 0;
            Balance_User balance = value as Balance_User;
            finalBalance = System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture);
            
            if (finalBalance > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            else if (finalBalance == 0)
                colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;
            else
                colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
