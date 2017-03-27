using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            catch (FormatException)
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
                result = username + " paid " + FormatCurrency(currency, paidShare);

                if (owedShare > 0)
                {
                    result += " and owes " + FormatCurrency(currency, owedShare);
                }

            }
            else
            {
                result = username + " owes " + FormatCurrency(currency, owedShare);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private string FormatCurrency(string currency_code, double value)
        {
            if (currency_code.Equals(App.currentUser.default_currency))
            {
                QueryDatabase obj = new QueryDatabase();
                string unit = obj.GetUnitForCurrency(currency_code);
                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                format.CurrencySymbol = unit;
                format.CurrencyNegativePattern = 1;
                return String.Format(format, "{0:C}", value);
            }
            else
            {
                return currency_code + String.Format("{0:0.00}", value);
            }
        }
    }
}
