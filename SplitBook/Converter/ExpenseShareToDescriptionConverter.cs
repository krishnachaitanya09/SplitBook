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
    public class ExpenseShareToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Expense_Share> users = value as List<Expense_Share>;

            Expense_Share currentUser = null;
            foreach (var user in users)
            {
                if (user.user_id == Helpers.getCurrentUserId())
                {
                    currentUser = user;
                    break;
                }
            }

            string description = null;
            if (currentUser == null || System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture) == 0)
                description = "not involved";

            else if (System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture) > 0)
                description = "you lent";
            else if (System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture) < 0)
                description = "you borrowed";

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
