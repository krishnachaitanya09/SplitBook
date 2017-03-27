using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SplitBook.Converter
{
    public class ExpenseShareToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush colorBrush;
            bool expenseDetail = false;
            if (parameter!=null && String.Equals(parameter.ToString(), "expenseDetail"))
            {
                expenseDetail = true;
            }
            List<Expense_Share> users = value as List<Expense_Share>;

            Expense_Share currentUser = null;
            foreach (var user in users)
            {
                if (user.user_id == Helpers.GetCurrentUserId())
                {
                    currentUser = user;
                    break;
                }
            }

            if (currentUser == null)
            {
                if(expenseDetail)
                    colorBrush = Application.Current.Resources["splitwiseGreyBG"] as SolidColorBrush;
                else
                    colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;
            }

            else if (System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture) > 0)
            {
                if (expenseDetail)
                    colorBrush = Application.Current.Resources["positiveLight"] as SolidColorBrush;
                else
                    colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            }

            else if (System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (expenseDetail)
                    colorBrush = Application.Current.Resources["splitwiseGreyBG"] as SolidColorBrush;
                else
                    colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;
            }

            else
            {
                if (expenseDetail)
                    colorBrush = Application.Current.Resources["negativeLight"] as SolidColorBrush;
                else
                    colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;
            }

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
