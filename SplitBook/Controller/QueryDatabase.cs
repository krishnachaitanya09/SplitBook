using Microsoft.EntityFrameworkCore;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Controller
{
    class QueryDatabase
    {
        private static int EXPENSES_ROWS = 10;

        //Returns the list of friends along with the balance and their picture.
        public List<User> getAllFriends()
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<User> friendsList = db.User.Include(u=>u.picture).Include(u => u.balance).Where(u=>u.id != Helpers.getCurrentUserId()).OrderBy(u=>u.first_name).ToList<User>();
                App.currentUser = db.User.Include(u => u.picture).Where(u => u.id == Helpers.getCurrentUserId()).FirstOrDefault();                               
                return friendsList;
            }
        }

        public User getCurrentUser()
        {
            using (SplitBookContext db = new SplitBookContext())
            {               
                return db.User.Include(u => u.picture).Where(u => u.id == Helpers.getCurrentUserId()).FirstOrDefault();
            }
        }

        public List<User> getAllUsersIncludingMyself()
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<User> friendsList = db.User.OrderBy(u=>u.first_name).ToList<User>();
                return friendsList;
            }
        }

        public List<Expense> getAllExpenses(int pageNo = 0)
        {
            int offset = EXPENSES_ROWS * pageNo;

            using (SplitBookContext db = new SplitBookContext())
            {
                //Only retrieve expenses that have not been deleted
                List<Expense> expensesList = db.Expense.Include(e => e.repayments).Include(e => e.users).Where(e=>e.deleted_by_user_id == 0).OrderByDescending(e=>e.date).Skip(offset).Take(EXPENSES_ROWS).ToList<Expense>();

                if (expensesList == null && expensesList.Count == 0)
                    return null;

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                    //expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    //if (expensesList[x].updated_by_user_id != 0)
                    //    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    //if (expensesList[x].deleted_by_user_id != 0)
                    //    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    //expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                    }
                }
                return expensesList;
            }
        }

        public List<Expense> getExpensesForUser(int userId, int pageNo = 0)
        {
            int offset = EXPENSES_ROWS * pageNo;

            //the expenses for for a user is a combination of expenses paid by him and owe by me
            //or
            //paid by me and owed by him
            //Only retrieve expenses that have not been deleted
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Expense> expensesList = db.Expense.Include(e => e.repayments).Include(e=>e.users)
                    .Where(e => e.deleted_by_user_id == 0 & e.repayments.All(r => (r.from == Helpers.getCurrentUserId() && r.to == userId) || (r.from == userId && r.to == Helpers.getCurrentUserId())))
                    .OrderByDescending(e => e.date).Skip(offset).Take(EXPENSES_ROWS).ToList();
               
                if (expensesList == null && expensesList.Count == 0)
                    return null;

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    //display amount details specific to user
                    expensesList[x].displayType = Expense.DISPLAY_FOR_SPECIFIC_USER;
                    expensesList[x].specificUserId = userId;

                    //expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    //if (expensesList[x].updated_by_user_id != 0)
                    //    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    //if (expensesList[x].deleted_by_user_id != 0)
                    //    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    //expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                    }
                }
                return expensesList;
            }
        }

        public List<Group> getAllGroups()
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Group> groupsList = db.Group.Include(g=>g.group_members).Include(g=>g.simplified_debts).Where(g=>g.id != 0).OrderBy(g=>g.name).ToList<Group>();
                if (groupsList != null)
                {
                    for (var x = 0; x < groupsList.Count; x++)
                    {
                        groupsList[x].members = new List<User>();
                        //List<Group_Members> groupMembers = dbConn.Query<Group_Members>("SELECT * FROM group_members WHERE group_id= ?", param).ToList<Group_Members>();
                        foreach (var group_member in groupsList[x].group_members)
                        {
                            groupsList[x].members.Add(getUserDetails(group_member.user_id));
                        }

                        foreach (var groupDebt in groupsList[x].simplified_debts)
                        {
                            groupDebt.fromUser = getUserDetails(groupDebt.from);
                            groupDebt.toUser = getUserDetails(groupDebt.to);

                            //groupsList[x].simplified_debts.Add(groupDebt);
                        }
                    }
                }
                return groupsList;
            }
        }

        public List<Expense> getAllExpensesForGroup(int groupId, int pageNo = 0)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                int offset = EXPENSES_ROWS * pageNo;

                //Only retrieve expenses that have not been deleted
                List<Expense> expensesList = db.Expense.Include(e => e.repayments).Include(e => e.users).Where(e=>e.deleted_by_user_id == 0 && e.group_id==groupId).OrderByDescending(e=>e.date).Skip(offset).Take(EXPENSES_ROWS).ToList<Expense>();

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                    //expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    //if (expensesList[x].updated_by_user_id != 0)
                    //    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    //if (expensesList[x].deleted_by_user_id != 0)
                    //    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    //expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                    }
                }

                return expensesList;
            }
        }

        public List<Currency> getSupportedCurrencies()
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Currency> currencyList = db.Currency.OrderBy(c=>c.currency_code).ToList();
                return currencyList;
            }
        }

        public string getUnitForCurrency(string currencyCode)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Currency> currencyList = db.Currency.Where(c=>c.currency_code == currencyCode).ToList();
                if (currencyList != null && currencyList.Count != 0)
                {
                    Currency currency = currencyList.First();
                    return currency.unit;
                }
                else
                    return currencyCode;
            }
        }

        public List<Expense> searchForExpense(string searchText)
        {           
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Expense> expensesList = db.Expense.Include(e => e.repayments).Include(e => e.users).Where(e => e.deleted_by_user_id == 0 && (e.description.ToUpper()).Contains(searchText)).OrderByDescending(e => e.date).Take(15).ToList();

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                    //expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    //if (expensesList[x].updated_by_user_id != 0)
                    //    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    //if (expensesList[x].deleted_by_user_id != 0)
                    //    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    //expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                    }
                }

                return expensesList;
            }
        }

        public List<Balance_User> getUserBalance(int userId)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                return db.Balance_User.Where(b => b.user_id == userId && b.amount != "0.0" && b.amount != "-0.0").ToList();                
            }
        }

        private List<Expense_Share> getExpenseShareUsers(int expenseId, string currencyCode)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Expense_Share> expenseShareList = db.Expense_Share.Where(e => e.expense_id == expenseId).ToList();                   

                for (var y = 0; y < expenseShareList.Count; y++)
                {
                    expenseShareList[y].currency = currencyCode;
                }
                return expenseShareList;
            }
        }

        private List<Debt_Expense> getExpenseRepayments(int expenseId)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                List<Debt_Expense> debtExpensesList = db.Debt_Expense.Where(d => d.expense_id == expenseId).ToList();                    

                for (var y = 0; y < debtExpensesList.Count; y++)
                {
                    debtExpensesList[y].fromUser = getUserDetails(debtExpensesList[y].from);
                    debtExpensesList[y].toUser = getUserDetails(debtExpensesList[y].to);
                }
                return debtExpensesList;
            }
        }

        private User getUserDetails(int userId)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                return db.User.Include(u=>u.picture).Where(u => u.id == userId).FirstOrDefault();                                   
            }
        }

        private Picture getUserPicture(int userId)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                return db.Picture.Where(p => p.user_id == userId).FirstOrDefault();                    
            }
        }
    }
}
