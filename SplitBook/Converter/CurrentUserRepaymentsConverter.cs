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
    public class CurrentUserRepaymentsConverter : IValueConverter
    {
        Expense expense;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            expense = value as Expense;
            if (expense == null)
                return "You";

            List<Expense_Share> users = expense.users;

            Expense_Share currentUser = null;
            if (users != null)
            {
                foreach (var user in users)
                {
                    if (user.user_id == Helpers.GetCurrentUserId())
                    {
                        currentUser = user;
                        break;
                    }
                }
            }
            if (currentUser == null)
                return "You owe";

            if (Helpers.IsEmpty(currentUser.net_balance))
                return "You owe";

            double netBalance = System.Convert.ToDouble(currentUser.net_balance, System.Globalization.CultureInfo.InvariantCulture);
            
            if(netBalance == 0)
                return "You owe";
            
            //string amount = expense.currency_code + String.Format("{0:0.00}", Math.Abs(netBalance));

            if (netBalance > 0)
            {
                int peopleWhoOweYou = getNumberofPeopleWhoOweYou();
               
                if (peopleWhoOweYou > 1)
                {
                    return peopleWhoOweYou.ToString() + " people owe you"; //+ amount + " for this.";
                }
                else if (expense.payment)
                {
                    User user = getUserWhoOwesYou();
                    return "You paid " + user.name;// + amount;
                }
                else
                {
                    User user = getUserWhoOwesYou();
                    if (user == null)
                        return "You paid";
                    else
                        return user.name + " owes you";// + amount + " for this.";
                }
            }
            else
            {
                int peopleYouOweTo = getNumberofPeopleYouOweTo();
                if (peopleYouOweTo > 1)
                {
                    return "You owe " + peopleYouOweTo.ToString() + " people";// + amount + " for this.";
                }
                else if (expense.payment)
                {
                    User user = getUserWhoYouOweTo();
                    return user.name + " paid you";// + amount;
                }
                else
                {
                    User user = getUserWhoYouOweTo();
                    if (user == null)
                        return "You owe";
                    else
                        
                    return "You owe " + user.name;// + amount + " for this.";
                }
            }
        }

        private User getUserWhoOwesYou()
        {
            int currentUserId = Helpers.GetCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.to == currentUserId)
                {
                    return repayment.fromUser;
                }
            }
            return null;
        }

        private User getUserWhoYouOweTo()
        {
            int currentUserId = Helpers.GetCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.from == currentUserId)
                {
                    return repayment.toUser;
                }
            }
            return null;
        }

        private int getNumberofPeopleWhoOweYou()
        {
            int peopleWhoOweYou = 0;
            int currentUserId = Helpers.GetCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.to == currentUserId)
                {
                    peopleWhoOweYou++;
                }
            }
            return peopleWhoOweYou;
        }

        private int getNumberofPeopleYouOweTo()
        {
            int peopleYouOweTo = 0;
            int currentUserId = Helpers.GetCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.from == currentUserId)
                {
                    peopleYouOweTo++;
                }
            }
            return peopleYouOweTo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
