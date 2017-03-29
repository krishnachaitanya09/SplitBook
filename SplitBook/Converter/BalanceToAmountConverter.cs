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
            if (value != null)
            {
                if (parameter != null && parameter.ToString().Equals("overall"))
                {
                    QueryDatabase query = new QueryDatabase();
                    string currencyCode = String.Empty;
                    string formatedValue = String.Empty;
                    if (App.currentUser != null)
                        currencyCode = query.GetUnitForCurrency(App.currentUser.default_currency.ToUpper());
                    string[] valueSplit = value.ToString().Split('*');
                    bool hasMultipleBalances = valueSplit.Length > 1;
                    if (currencyCode != String.Empty)
                    {
                        var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                        format.CurrencySymbol = currencyCode;
                        format.CurrencyNegativePattern = 1;
                        formatedValue = String.Format(format, "{0:C}", System.Convert.ToDouble(valueSplit[0]));
                    }
                    else
                    {
                        formatedValue = valueSplit[0].ToString();
                    }
                    if (hasMultipleBalances)
                        formatedValue += '*';
                    return formatedValue;
                }
                else
                {
                    List<Balance_User> balanceList = value as List<Balance_User>;
                    bool hasMultipleBalances = Helpers.HasMultipleBalances(balanceList);

                    Balance_User defaultBalance = Helpers.GetDefaultBalance(balanceList);
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
                            string unit = obj.GetUnitForCurrency(currency);
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
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
