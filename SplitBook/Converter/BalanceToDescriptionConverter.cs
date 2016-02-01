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
    public class BalanceToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string description = "settled up";
            double finalBalance = 0;
            if (parameter != null && parameter.ToString().Equals("group"))
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
                description = "owes you";
            else if (finalBalance < 0)
                description = "you owe";

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
