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
    public class ExpenseShareToAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Expense expense = value as Expense;
            List<Expense_Share> users = expense.users;

            Expense_Share currentUser = null;
            foreach (var user in users)
            {
                if (user.user_id == Helpers.getCurrentUserId())
                {
                    currentUser = user;
                    break;
                }
            }

            double amount = System.Convert.ToDouble("0.00", System.Globalization.CultureInfo.InvariantCulture);
            QueryDatabase obj = new QueryDatabase();
            string unit = obj.getUnitForCurrency(expense.currency_code);
            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencySymbol = unit;
            format.CurrencyNegativePattern = 1;

            if (currentUser == null)
            {
                amount = System.Convert.ToDouble("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (expense.displayType == Expense.DISPLAY_FOR_ALL_USER)
                amount = Math.Abs(System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture));
            else
            {
                List<Debt_Expense> repayments = expense.repayments;
                int currentUserId = Helpers.getCurrentUserId();
                int specificUserId = expense.specificUserId;
                foreach (var repayment in repayments)
                {
                    if ((repayment.from == currentUserId && repayment.to == specificUserId) || (repayment.to == currentUserId && repayment.from == specificUserId))
                    {
                        amount = Math.Abs(System.Convert.ToDouble(repayment.amount, System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }
                }
            }

            if (expense.currency_code.Equals(App.currentUser.default_currency))
            {
                return String.Format(format, "{0:C}", amount);
            }
            else
            {
                return expense.currency_code + String.Format("{0:0.00}", amount);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
