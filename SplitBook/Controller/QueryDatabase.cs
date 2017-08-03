using Microsoft.HockeyApp;
using SplitBook.Model;
using SplitBook.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Controller
{
    class QueryDatabase
    {
        private static int EXPENSES_ROWS = 10;

        //Returns the list of friends along with the balance and their picture.
        public List<User> GetAllFriends()
        {
            List<User> friendsList = new List<User>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    friendsList = dbConn.Query<User>("SELECT * FROM user ORDER BY first_name").ToList<User>();
                    //remove the current user from the list as the user table also contains his details.
                    for (var x = 0; x < friendsList.Count; x++)
                    {
                        if (friendsList[x].id == Helpers.GetCurrentUserId())
                        {
                            if (App.currentUser == null)
                            {
                                App.currentUser = friendsList[x];
                                HockeyClient.Current.UpdateContactInfo(App.currentUser.name, App.currentUser.email);
                                App.currentUser.picture = GetUserPicture(friendsList[x].id, dbConn);
                            }
                            friendsList.Remove(friendsList[x]);
                            //As one element has been removed
                            x--;
                            continue;
                        }

                        object[] param = { friendsList[x].id };
                        friendsList[x].balance = dbConn.Query<Balance_User>("SELECT * FROM balance_user WHERE user_id= ?  AND amount <> '0.0' AND amount <> '-0.0'", param).ToList<Balance_User>();
                        friendsList[x].picture = GetUserPicture(friendsList[x].id, dbConn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return friendsList;
        }

        //public User getCurrentUser()
        //{
        //    using (SplitBookContext db = new SplitBookContext())
        //    {
        //        return db.User.Include(u => u.picture).Where(u => u.id == Helpers.getCurrentUserId()).FirstOrDefault();
        //    }
        //}

        //public List<User> getAllUsersIncludingMyself()
        //{
        //    using (SplitBookContext db = new SplitBookContext())
        //    {
        //        List<User> friendsList = db.User.OrderBy(u => u.first_name).ToList<User>();
        //        return friendsList;
        //    }
        //}

        public List<Expense> GetAllExpenses(int pageNo = 0)
        {
            int offset = EXPENSES_ROWS * pageNo;
            object[] param = { offset, EXPENSES_ROWS };
            List<Expense> expensesList = new List<Expense>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    //Only retrieve expenses that have not been deleted
                    expensesList = dbConn.Query<Expense>("SELECT * FROM expense WHERE deleted_by_user_id=0 ORDER BY datetime(date) DESC LIMIT ?,?", param).ToList<Expense>();

                    if (expensesList == null && expensesList.Count == 0)
                        return null;

                    //Get list of repayments for expense.
                    //Get the created by, updated by and deleted by user
                    //Get the expense share per user. Within each expense user, fill in the user details.
                    for (var x = 0; x < expensesList.Count; x++)
                    {
                        expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                        expensesList[x].repayments = GetExpenseRepayments(expensesList[x].id, dbConn);
                        expensesList[x].created_by = GetUserDetails(expensesList[x].created_by_user_id, dbConn);

                        if (expensesList[x].updated_by_user_id != 0)
                            expensesList[x].updated_by = GetUserDetails(expensesList[x].updated_by_user_id, dbConn);

                        if (expensesList[x].deleted_by_user_id != 0)
                            expensesList[x].deleted_by = GetUserDetails(expensesList[x].deleted_by_user_id, dbConn);

                        expensesList[x].users = GetExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code, dbConn);

                        for (var y = 0; y < expensesList[x].users.Count; y++)
                        {
                            expensesList[x].users[y].user = GetUserDetails(expensesList[x].users[y].user_id, dbConn);
                        }
                        expensesList[x].receipt = GetReceipt(expensesList[x].id, dbConn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return expensesList;
        }

        public List<Expense> GetExpensesForUser(int userId, int pageNo = 0)
        {
            int offset = EXPENSES_ROWS * pageNo;

            //the expenses for for a user is a combination of expenses paid by him and owe by me
            //or
            //paid by me and owed by him
            //Only retrieve expenses that have not been deleted
            object[] param = { Helpers.GetCurrentUserId(), userId, userId, Helpers.GetCurrentUserId(), offset, EXPENSES_ROWS };
            List<Expense> expensesList = new List<Expense>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    expensesList = dbConn.Query<Expense>("SELECT expense.id, expense.group_id, expense.description, expense.details, expense.payment, expense.transaction_confirmed, expense.creation_method, expense.cost, expense.currency_code, expense.date, expense.created_by_user_id, expense.created_at, expense.updated_by_user_id, expense.updated_at, expense.deleted_at, expense.deleted_by_user_id FROM expense INNER JOIN debt_expense ON expense.id = debt_expense.expense_id WHERE expense.deleted_by_user_id=0 AND ((debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?) OR (debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?)) ORDER BY datetime(date) DESC LIMIT ?,?", param).ToList<Expense>();

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

                        expensesList[x].repayments = GetExpenseRepayments(expensesList[x].id, dbConn);
                        expensesList[x].created_by = GetUserDetails(expensesList[x].created_by_user_id, dbConn);

                        if (expensesList[x].updated_by_user_id != 0)
                            expensesList[x].updated_by = GetUserDetails(expensesList[x].updated_by_user_id, dbConn);

                        if (expensesList[x].deleted_by_user_id != 0)
                            expensesList[x].deleted_by = GetUserDetails(expensesList[x].deleted_by_user_id, dbConn);

                        expensesList[x].users = GetExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code, dbConn);

                        for (var y = 0; y < expensesList[x].users.Count; y++)
                        {
                            expensesList[x].users[y].user = GetUserDetails(expensesList[x].users[y].user_id, dbConn);
                        }

                        expensesList[x].receipt = GetReceipt(expensesList[x].id, dbConn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return expensesList;
        }

        public List<Group> GetAllGroups()
        {
            List<Group> groupsList = new List<Group>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    groupsList = dbConn.Query<Group>("SELECT * FROM [group] ORDER BY name").ToList<Group>();
                    if (groupsList != null)
                    {
                        for (var x = 0; x < groupsList.Count; x++)
                        {
                            groupsList[x].members = new List<User>();
                            groupsList[x].simplified_debts = new List<Debt_Group>();

                            object[] param = { groupsList[x].id };
                            List<Group_Members> groupMembers = dbConn.Query<Group_Members>("SELECT * FROM group_members WHERE group_id= ?", param).ToList<Group_Members>();
                            foreach (var member in groupMembers)
                            {
                                groupsList[x].members.Add(GetUserDetails(member.user_id, dbConn));
                            }

                            List<Debt_Group> groupSimplifiedDebts = dbConn.Query<Debt_Group>("SELECT * FROM debt_group WHERE group_id= ?", param).ToList<Debt_Group>();
                            foreach (var groupDebt in groupSimplifiedDebts)
                            {
                                groupDebt.fromUser = GetUserDetails(groupDebt.from, dbConn);
                                groupDebt.toUser = GetUserDetails(groupDebt.to, dbConn);

                                groupsList[x].simplified_debts.Add(groupDebt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return groupsList;
        }

        public List<Expense> GetAllExpensesForGroup(int groupId, int pageNo = 0)
        {
            List<Expense> expensesList = new List<Expense>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    int offset = EXPENSES_ROWS * pageNo;
                    object[] param = { groupId, offset, EXPENSES_ROWS };

                    //Only retrieve expenses that have not been deleted
                    expensesList = dbConn.Query<Expense>("SELECT * FROM expense WHERE deleted_by_user_id=0 AND group_id=? ORDER BY datetime(date) DESC LIMIT ?,?", param).ToList<Expense>();

                    //Get list of repayments for expense.
                    //Get the created by, updated by and deleted by user
                    //Get the expense share per user. Within each expense user, fill in the user details.
                    for (var x = 0; x < expensesList.Count; x++)
                    {
                        expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                        expensesList[x].repayments = GetExpenseRepayments(expensesList[x].id, dbConn);
                        expensesList[x].created_by = GetUserDetails(expensesList[x].created_by_user_id, dbConn);

                        if (expensesList[x].updated_by_user_id != 0)
                            expensesList[x].updated_by = GetUserDetails(expensesList[x].updated_by_user_id, dbConn);

                        if (expensesList[x].deleted_by_user_id != 0)
                            expensesList[x].deleted_by = GetUserDetails(expensesList[x].deleted_by_user_id, dbConn);

                        expensesList[x].users = GetExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code, dbConn);

                        for (var y = 0; y < expensesList[x].users.Count; y++)
                        {
                            expensesList[x].users[y].user = GetUserDetails(expensesList[x].users[y].user_id, dbConn);
                        }
                        expensesList[x].receipt = GetReceipt(expensesList[x].id, dbConn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return expensesList;
        }

        public List<Currency> GetSupportedCurrencies()
        {
            List<Currency> currencyList = new List<Currency>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    currencyList = dbConn.Query<Currency>("SELECT * FROM currency ORDER BY currency_code");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return currencyList;
        }

        public string GetUnitForCurrency(string currencyCode)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
            {
                try
                {
                    List<Currency> currencyList = dbConn.Query<Currency>("SELECT * FROM currency WHERE currency_code = ?", new Object[] { currencyCode });
                    if (currencyList != null && currencyList.Count != 0)
                    {
                        Currency currency = currencyList.FirstOrDefault();
                        return currency.unit;
                    }
                    else
                        return currencyCode;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return currencyCode;
                }
            }
        }

        public List<Expense> SearchForExpense(string searchText)
        {
            //int offset = EXPENSES_ROWS * pageNo;
            object[] param = { 15 }; //top 15 results only

            //Only retrieve expenses that have not been deleted
            string query = "SELECT * FROM expense WHERE deleted_by_user_id=0 AND upper(description) LIKE upper('%" + searchText + "%') ORDER BY datetime(date) DESC LIMIT ?";
            List<Expense> expensesList = new List<Expense>();
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
                {
                    expensesList = dbConn.Query<Expense>(query, param).ToList<Expense>();

                    //Get list of repayments for expense.
                    //Get the created by, updated by and deleted by user
                    //Get the expense share per user. Within each expense user, fill in the user details.
                    for (var x = 0; x < expensesList.Count; x++)
                    {
                        expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                        expensesList[x].repayments = GetExpenseRepayments(expensesList[x].id, dbConn);
                        expensesList[x].created_by = GetUserDetails(expensesList[x].created_by_user_id, dbConn);

                        if (expensesList[x].updated_by_user_id != 0)
                            expensesList[x].updated_by = GetUserDetails(expensesList[x].updated_by_user_id, dbConn);

                        if (expensesList[x].deleted_by_user_id != 0)
                            expensesList[x].deleted_by = GetUserDetails(expensesList[x].deleted_by_user_id, dbConn);

                        expensesList[x].users = GetExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code, dbConn);

                        for (var y = 0; y < expensesList[x].users.Count; y++)
                        {
                            expensesList[x].users[y].user = GetUserDetails(expensesList[x].users[y].user_id, dbConn);
                        }
                        expensesList[x].receipt = GetReceipt(expensesList[x].id, dbConn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return expensesList;
        }

        public List<Balance_User> GetUserBalance(int userId)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH))
            {
                object[] param = { userId };
                return dbConn.Query<Balance_User>("SELECT * FROM balance_user WHERE user_id= ?  AND amount <> '0.0' AND amount <> '-0.0'", param).ToList<Balance_User>();
            }
        }

        private List<Expense_Share> GetExpenseShareUsers(int expenseId, string currencyCode, SQLiteConnection dbConn)
        {
            List<Expense_Share> expenseShareList = dbConn.Query<Expense_Share>("SELECT * FROM expense_share WHERE expense_id= ?", new object[] { expenseId }).ToList<Expense_Share>();

            for (var y = 0; y < expenseShareList.Count; y++)
            {
                expenseShareList[y].currency = currencyCode;
            }
            return expenseShareList;
        }

        private List<Debt_Expense> GetExpenseRepayments(int expenseId, SQLiteConnection dbConn)
        {
            List<Debt_Expense> debtExpensesList = dbConn.Query<Debt_Expense>("SELECT * FROM debt_expense WHERE expense_id= ?", new object[] { expenseId }).ToList<Debt_Expense>();

            for (var y = 0; y < debtExpensesList.Count; y++)
            {
                debtExpensesList[y].fromUser = GetUserDetails(debtExpensesList[y].from, dbConn);
                debtExpensesList[y].toUser = GetUserDetails(debtExpensesList[y].to, dbConn);
            }
            return debtExpensesList;
        }

        private User GetUserDetails(int userId, SQLiteConnection dbConn)
        {
            User user = dbConn.Query<User>("SELECT * FROM user WHERE id= ?", new object[] { userId }).FirstOrDefault();
            if (user != null)
            {
                user.picture = GetUserPicture(userId, dbConn);
                return user;
            }
            else
                return null;
        }

        private Picture GetUserPicture(int userId, SQLiteConnection dbConn)
        {
            return dbConn.Query<Picture>("SELECT * FROM picture WHERE user_id= ?", new object[] { userId }).FirstOrDefault();
        }

        private Receipt GetReceipt(int expenseId, SQLiteConnection dbConn)
        {
            return dbConn.Query<Receipt>("SELECT * FROM receipt WHERE expense_id=?", new object[] { expenseId }).FirstOrDefault() ?? new Receipt();
        }
    }
}
