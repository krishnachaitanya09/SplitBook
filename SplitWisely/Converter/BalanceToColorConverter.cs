using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SplitWisely.Converter
{
    public class BalanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush colorBrush;
            double finalBalance = 0;
            if(parameter != null && parameter.ToString().Equals("overall"))
            {
                finalBalance += System.Convert.ToDouble(value);
            }
            else if (parameter != null && parameter.ToString().Equals("group"))
            {
                List<Debt_Group> allDebts = value as List<Debt_Group>;
                finalBalance = Helpers.getUserGroupDebtAmount(allDebts, App.currentUser.id);
            }
            else
            {
                List<Balance_User> balance = value as List<Balance_User>;
                Balance_User defaultBalance = Helpers.getDefaultBalance(balance);
                finalBalance = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (finalBalance > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            else if(finalBalance == 0)
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
