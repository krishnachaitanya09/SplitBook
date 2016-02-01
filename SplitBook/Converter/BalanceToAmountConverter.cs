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
    public class BalanceToAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
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

                //QueryDatabase obj = new QueryDatabase();
                //string unit = obj.getUnitForCurrency(currency);

                string amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
                if (hasMultipleBalances)
                    return amount + "*";
                else
                    return amount;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
