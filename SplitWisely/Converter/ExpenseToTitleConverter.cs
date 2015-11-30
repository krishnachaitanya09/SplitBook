using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitWisely.Converter
{
    public class ExpenseToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Expense expense = value as Expense;
                if (!expense.payment)
                    return expense.description;
                else
                {
                    List<Expense_Share> users = expense.users;
                    string paid = " paid";
                    string amount = getPaidByUser(users, language).paid_share;
                    return getPaidByUser(users, language).user.first_name + paid + " " + getPaidToUser(users, language).user.first_name + " " + expense.currency_code + System.Convert.ToDouble(amount, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                return "Unable to get description";
            }
        }

        private Expense_Share getPaidByUser(List<Expense_Share> users, string language)
        {
            foreach (var expenseUser in users)
            {
                if (System.Convert.ToDouble(expenseUser.paid_share) > 0)
                {
                    return expenseUser;
                }
            }

            return null;
        }

        private Expense_Share getPaidToUser(List<Expense_Share> users, string language)
        {
            foreach (var expenseUser in users)
            {
                if (System.Convert.ToDouble(expenseUser.paid_share) == 0)
                {
                    return expenseUser;
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
