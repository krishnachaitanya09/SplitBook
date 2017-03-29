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
    public class ExpenseShareToWhoPaidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Expense expense = value as Expense;
            List<Expense_Share> users = expense.users;

            Expense_Share paidUser = null;
            foreach (var user in users)
            {
                if (System.Convert.ToDouble(user.paid_share, System.Globalization.CultureInfo.InvariantCulture) > 0)
                {
                    paidUser = user;
                    break;
                }
            }

            if (checkIfNotInvolved(users))
            {
                return "You are not involved";
            }

            if (paidUser == null)
                return null;

            string amount = null;
            string currency = expense.currency_code;
            QueryDatabase obj = new QueryDatabase();
            string unit = obj.GetUnitForCurrency(currency);

            if (!String.IsNullOrEmpty(unit))
            {
                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                format.CurrencySymbol = unit;
                format.CurrencyNegativePattern = 1;
                amount = String.Format(format, "{0:C}", Math.Abs(System.Convert.ToDouble(paidUser.paid_share, CultureInfo.InvariantCulture)));
            }
            else
            {
                amount = expense.currency_code + String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(expense.cost, CultureInfo.InvariantCulture)));
            }

            string paid = " paid";

            return getPaidUserName(paidUser.user) + paid + " " + amount;
        }

        private String getPaidUserName(User paidUser)
        {
            if (paidUser == null)
                return "Unknown user (not a friend)";

            if (paidUser.id == Helpers.GetCurrentUserId())
                return "You";
            else
            {
                if (paidUser.first_name != null)
                    return paidUser.first_name;
                else
                    return "Unknown user (not a friend)";
            }
        }

        private bool checkIfNotInvolved(List<Expense_Share> users)
        {
            foreach (var user in users)
            {
                if (user.user_id == Helpers.GetCurrentUserId())
                {
                    return false;
                }
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
