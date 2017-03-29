using SplitBook.Controller;
using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitBook.Converter.UserDetails
{
    public class UserDetailsExpensesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string description = "settled up";
            double finalBalance = 0;
            string amount = String.Empty;
            QueryDatabase query = new QueryDatabase();
            Balance_User balance = value as Balance_User;
            finalBalance = System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture);
            string currency = balance.currency_code;
            QueryDatabase obj = new QueryDatabase();
            string unit = obj.GetUnitForCurrency(currency);
            if (!String.IsNullOrEmpty(unit))
            {
                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                format.CurrencySymbol = unit;
                format.CurrencyNegativePattern = 1;
                amount = String.Format(format, "{0:C}", Math.Abs(finalBalance));

            }
            else
            {
                amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
            }

            if (finalBalance > 0)
                description = "owes you " + amount;
            else if (finalBalance < 0)
                description = "you owe " + amount;

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
