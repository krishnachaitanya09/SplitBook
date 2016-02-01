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

            string amount = null;
            if (currentUser == null)
                return expense.currency_code + String.Format("{0:0.00}", System.Convert.ToDouble("0.00", System.Globalization.CultureInfo.InvariantCulture));

            if(expense.displayType == Expense.DISPLAY_FOR_ALL_USER)
                amount = String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture)));

            else
            {
                List<Debt_Expense> repayments = expense.repayments;
                int currentUserId = Helpers.getCurrentUserId();
                int specificUserId = expense.specificUserId;
                foreach (var repayment in repayments)
                {
                    if ((repayment.from == currentUserId && repayment.to == specificUserId) || (repayment.to == currentUserId && repayment.from == specificUserId))
                    {
                        amount = String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(repayment.amount, System.Globalization.CultureInfo.InvariantCulture)));
                        break;
                    }
                }
            }

            return expense.currency_code + amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
