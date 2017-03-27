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
    public class GroupDebtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string amount = "";
            List<Debt_Group> allDebts = value as List<Debt_Group>;
            double finalBalance = Helpers.GetUserGroupDebtAmount(allDebts, App.currentUser.id);

            //if final balance is 0, then anyways we are not shwoing the balance.
            if (finalBalance != 0)
            {
                string currency = allDebts[0].currency_code;
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
            }
            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
