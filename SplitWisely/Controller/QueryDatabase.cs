using SplitWisely.Model;
using SplitWisely.Utilities;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Controller
{
    class QueryDatabase
    {
        private static int EXPENSES_ROWS = 10;

        //Returns the list of friends along with the balance and their picture.
        public List<User> getAllFriends()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<User> friendsList = dbConn.GetAllWithChildren<User>().OrderBy(u => u.first_name).ToList();
                //remove the current user from the list as the user table also contains his details.
                for (var x = 0; x < friendsList.Count; x++)
                {
                    if (friendsList[x].id == Helpers.getCurrentUserId())
                    {
                        if (App.currentUser == null)
                        {
                            App.currentUser = friendsList[x];
                            //App.currentUser.picture = getUserPicture(friendsList[x].id);
                        }
                        friendsList.Remove(friendsList[x]);
                        //As one element has been removed
                        x--;
                        continue;
                    }
                }
                return friendsList;
            }
        }

        public List<User> getAllUsersIncludingMyself()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<User> friendsList = dbConn.Query<User>("SELECT * FROM user ORDER BY first_name").ToList<User>();
                return friendsList;
            }
        }

        public List<Expense> getAllExpenses(int pageNo = 0)
        {
            int offset = EXPENSES_ROWS * pageNo;

            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                //Only retrieve expenses that have not been deleted
                List<Expense> expensesList = dbConn.GetAllWithChildren<Expense>(recursive: true).Where(e => e.deleted_by_user_id == 0).OrderBy(e => e.date).Skip(offset).Take(EXPENSES_ROWS).ToList();

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].currency = expensesList[x].currency_code;
                        //expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
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

            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Expense> expensesList = dbConn.GetAllWithChildren<Expense>().Where(e => e.deleted_by_user_id == 0 && e.repayments.Any(r => (r.from == Helpers.getCurrentUserId() && r.to == userId) || (r.from == userId && r.to == Helpers.getCurrentUserId()))).OrderByDescending(e => e.date).Skip(offset).Take(EXPENSES_ROWS).ToList();

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

                    for (var y = 0; y < expensesList[x].users.Count; y++)
                    {
                        expensesList[x].users[y].currency = expensesList[x].currency_code;
                        //expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                    }
                }
                return expensesList;
            }
        }

        //public List<Expense> getExpensesForUser(int userId, int pageNo = 0)
        //{
        //    int offset = EXPENSES_ROWS * pageNo;

        //    //the expenses for for a user is a combination of expenses paid by him and owe by me
        //    //or
        //    //paid by me and owed by him
        //    //Only retrieve expenses that have not been deleted

        //    object[] param = { Helpers.getCurrentUserId(), userId, userId, Helpers.getCurrentUserId(), offset, EXPENSES_ROWS };
        //    using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
        //    {
        //        List<Expense> expensesList = dbConn.Query<Expense>("SELECT expense.id, expense.group_id, expense.description, expense.details, expense.payment, expense.transaction_confirmed, expense.creation_method, expense.cost, expense.currency_code, expense.date, expense.created_by, expense.created_at, expense.updated_by, expense.updated_at, expense.deleted_at, expense.deleted_by FROM expense INNER JOIN debt_expense ON expense.id = debt_expense.expense_id WHERE expense.deleted_by=0 AND expense.group_id = 0 AND  ((debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?) OR (debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?)) ORDER BY datetime(date) DESC LIMIT ?,?", param).ToList<Expense>();

        //        if (expensesList == null && expensesList.Count == 0)
        //            return null;

        //        //Get list of repayments for expense.
        //        //Get the created by, updated by and deleted by user
        //        //Get the expense share per user. Within each expense user, fill in the user details.
        //        for (var x = 0; x < expensesList.Count; x++)
        //        {
        //            //display amount details specific to user
        //            expensesList[x].displayType = Expense.DISPLAY_FOR_SPECIFIC_USER;
        //            expensesList[x].specificUserId = userId;

        //            expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
        //            expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

        //            if (expensesList[x].updated_by_user_id != 0)
        //                expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

        //            if (expensesList[x].deleted_by_user_id != 0)
        //                expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

        //            expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

        //            for (var y = 0; y < expensesList[x].users.Count; y++)
        //            {
        //                expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
        //            }
        //        }

        //        return expensesList;
        //    }
        //}

        public List<Group> getAllGroups()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Group> groupsList = dbConn.GetAllWithChildren<Group>().Where(g => g.id != 0).OrderBy(g => g.name).ToList();
                return groupsList;
            }
        }

        public List<Expense> getAllExpensesForGroup(int groupId, int pageNo = 0)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                int offset = EXPENSES_ROWS * pageNo;

                //Only retrieve expenses that have not been deleted
                List<Expense> expensesList = dbConn.GetAllWithChildren<Expense>().Where(e => e.deleted_by_user_id == 0 && e.group_id == groupId).OrderBy(e => e.date).Skip(offset).Take(EXPENSES_ROWS).ToList();

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                    expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    if (expensesList[x].updated_by_user_id != 0)
                        expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    if (expensesList[x].deleted_by_user_id != 0)
                        expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

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
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Currency> currencyList = dbConn.Query<Currency>("SELECT * FROM currency ORDER BY currency_code");
                return currencyList;
            }
        }

        public string getUnitForCurrency(string currencyCode)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Currency> currencyList = dbConn.Query<Currency>("SELECT * FROM currency WHERE currency_code = ?", new Object[] { currencyCode });
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
            //int offset = EXPENSES_ROWS * pageNo;
            object[] param = { 15 }; //top 15 results only

            //Only retrieve expenses that have not been deleted
            string query = "SELECT * FROM expense WHERE deleted_by=0 AND upper(description) LIKE upper('%" + searchText + "%') ORDER BY datetime(date) DESC LIMIT ?";
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Expense> expensesList = dbConn.Query<Expense>(query, param).ToList<Expense>();

                //Get list of repayments for expense.
                //Get the created by, updated by and deleted by user
                //Get the expense share per user. Within each expense user, fill in the user details.
                for (var x = 0; x < expensesList.Count; x++)
                {
                    expensesList[x].displayType = Expense.DISPLAY_FOR_ALL_USER;
                    expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                    expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                    if (expensesList[x].updated_by_user_id != 0)
                        expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                    if (expensesList[x].deleted_by_user_id != 0)
                        expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                    expensesList[x].users = getExpenseShareUsers(expensesList[x].id, expensesList[x].currency_code);

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
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                object[] param = { userId };
                return dbConn.Query<Balance_User>("SELECT * FROM balance_user WHERE user_id= ?  AND amount <> '0.0' AND amount <> '-0.0'", param).ToList<Balance_User>();
            }
        }

        private List<Expense_Share> getExpenseShareUsers(int expenseId, string currencyCode)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Expense_Share> expenseShareList = dbConn.Query<Expense_Share>("SELECT * FROM expense_share WHERE expense_id= ?", new object[] { expenseId }).ToList<Expense_Share>();

                for (var y = 0; y < expenseShareList.Count; y++)
                {
                    expenseShareList[y].currency = currencyCode;
                }
                return expenseShareList;
            }
        }

        private List<Debt_Expense> getExpenseRepayments(int expenseId)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<Debt_Expense> debtExpensesList = dbConn.GetAllWithChildren<Debt_Expense>().Where(d => d.expense_id == expenseId).ToList();
                return debtExpensesList;
            }
        }

        private User getUserDetails(int userId)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                List<User> users = dbConn.GetAllWithChildren<User>().Where(u => u.id == userId).ToList();
                if (users != null && users.Count != 0)
                {
                    User user = users.First();
                    return user;
                }
                else
                    return null;
            }
        }

        private Picture getUserPicture(int userId)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true))
            {
                return dbConn.Query<Picture>("SELECT * FROM picture WHERE user_id= ?", new object[] { userId }).First();
            }
        }
    }
}
