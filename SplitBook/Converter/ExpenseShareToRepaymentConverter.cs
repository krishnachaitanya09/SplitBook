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
    public class ExpenseShareToRepaymentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Expense_Share shareUser = value as Expense_Share;
            string currency = shareUser.currency;
            double paidShare = 0, owedShare = 0;
            
            try
            {
                paidShare = System.Convert.ToDouble(shareUser.paid_share, System.Globalization.CultureInfo.InvariantCulture);
                owedShare = System.Convert.ToDouble(shareUser.owed_share, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException exception)
            {

            }
            
            string username;

            if (shareUser.user == null)
                username = "Unknown user (not a friend)";
            else
                username = shareUser.user.first_name;
            string result;

            if (paidShare > 0)
            {
                result = username + " paid " + currency + String.Format("{0:0.00}", paidShare);

                if (owedShare > 0)
                {
                    result += " and owes " + currency + String.Format("{0:0.00}", owedShare);
                }

            }
            else
            {
                result = username + " owes " + currency + String.Format("{0:0.00}", owedShare);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
