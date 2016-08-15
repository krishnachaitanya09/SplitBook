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
    public class BalanceToAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString().Equals("overall"))
            {
                QueryDatabase query = new QueryDatabase();
                string currencyCode = query.getUnitForCurrency(App.currentUser.default_currency.ToUpper());
                if (currencyCode != String.Empty)
                {
                    var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                    format.CurrencySymbol = currencyCode;
                    format.CurrencyNegativePattern = 1;
                    return String.Format(format, "{0:C}", System.Convert.ToDouble(value));

                }
                else
                {
                    return System.Convert.ToDouble(value).ToString();
                }
            }
            else
            {
                List<Balance_User> balanceList = value as List<Balance_User>;
                bool hasMultipleBalances = Helpers.hasMultipleBalances(balanceList);

                Balance_User defaultBalance = Helpers.getDefaultBalance(balanceList);
                double finalBalance = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
                if (finalBalance == 0)
                    return null;
                else
                {
                    string currency = defaultBalance.currency_code;
                    string amount;
                    if (currency.Equals(App.currentUser.default_currency))
                    {
                        QueryDatabase obj = new QueryDatabase();
                        string unit = obj.getUnitForCurrency(currency);
                        var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                        format.CurrencySymbol = unit;
                        format.CurrencyNegativePattern = 1;
                        amount = String.Format(format, "{0:C}", Math.Abs(finalBalance));
                    }
                    else
                    {
                        amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
                    }

                    if (hasMultipleBalances)
                        return amount + "*";
                    else
                        return amount;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
