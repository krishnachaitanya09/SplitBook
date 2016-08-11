using Microsoft.EntityFrameworkCore;
using SplitBook.Model;
using SplitBook.Request;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SplitBook.Controller
{
    class SyncDatabase
    {
        bool firstSync;
        Action<bool, HttpStatusCode> CallbackOnSuccess;

        public SyncDatabase(Action<bool, HttpStatusCode> callback)
        {
            this.CallbackOnSuccess = callback;
            firstSync = false;
        }

        public void isFirstSync(bool firstSync)
        {
            this.firstSync = firstSync;
        }

        public void performSync()
        {
            if (!Helpers.checkNetworkConnection())
            {
                CallbackOnSuccess(false, HttpStatusCode.ServiceUnavailable);
                return;
            }
            //if (firstSync)
            //{
            //    DeleteAllDataInDB();
            //}
            //Fetch current user details everytime to sync possible changes made on the website
            CurrentUserRequest request = new CurrentUserRequest();
            request.getCurrentUser(_CurrentUserDetailsReceived, _OnErrorReceived);

        }

        private void _CurrentUserDetailsReceived(User currentUser)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                db.RemoveRange(db.User);
                db.SaveChanges();
                db.User.Add(currentUser);
                db.SaveChanges();
            }

            //Save current user id in isolated storage
            Helpers.setCurrentUserId(currentUser.id);

            //Fire next request, i.e. get list of friends
            GetFriendsRequest request = new GetFriendsRequest();
            request.getAllFriends(_FriendsDetailsRecevied, _OnErrorReceived);
        }

        private void _FriendsDetailsRecevied(List<User> friendsList)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                foreach (var friend in friendsList)
                {
                    db.User.Add(friend);
                }
                db.SaveChanges();
            }
            //fetch expenses
            GetExpensesRequest request = new GetExpensesRequest();
            request.getAllExpenses(_ExpensesDetailsReceived, _OnErrorReceived);
        }


        private void _ExpensesDetailsReceived(List<Expense> expensesList)
        {
            if (expensesList == null || expensesList.Count == 0)
            {
                CallbackOnSuccess(true, HttpStatusCode.OK);
                return;
            }
            using (SplitBookContext db = new SplitBookContext())
            {
                db.RemoveRange(db.Expense);
                db.SaveChanges();
                //Insert expenses
                foreach (var expense in expensesList)
                {
                    //The api returns the entire user details of the created by, updated by and deleted by users.
                    //But we only need to store their id's into the database
                    if (expense.created_by != null)
                        expense.created_by_user_id = expense.created_by.id;

                    if (expense.updated_by != null)
                        expense.updated_by_user_id = expense.updated_by.id;

                    if (expense.deleted_by != null)
                        expense.deleted_by_user_id = expense.deleted_by.id;
                    db.Expense.Add(expense);
                }
                db.SaveChanges();
            }

            Helpers.setLastUpdatedTime();

            //Fetch groups
            GetGroupsRequest request = new GetGroupsRequest();
            request.getAllGroups(_GroupsDetailsReceived, _OnErrorReceived);
        }


        private void _GroupsDetailsReceived(List<Group> groupsList)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                db.Group.RemoveRange(db.Group);
                db.SaveChanges();
                foreach (var group in groupsList)
                {
                    group.group_members = new List<Group_Members>();
                    foreach (var member in group.members)
                    {
                        Group_Members group_member = new Group_Members();
                        group_member.group_id = group.id;
                        group_member.user_id = member.id;
                        group.group_members.Add(group_member);
                    }
                    db.Group.Add(group);
                }
                db.SaveChanges();
            }

            GetCurrenciesRequest request = new GetCurrenciesRequest();
            request.getSupportedCurrencies(_CurrenciesReceived);
        }

        private void _CurrenciesReceived(List<Currency> currencyList)
        {
            if (currencyList != null && currencyList.Count != 0)
            {
                using (SplitBookContext db = new SplitBookContext())
                {
                    db.Currency.RemoveRange(db.Currency);
                    db.SaveChanges();
                    foreach (Currency currency in currencyList)
                    {
                        db.Currency.Add(currency);
                    }
                    db.SaveChanges();
                }
            }
            CallbackOnSuccess(true, HttpStatusCode.OK);
        }

        private void _OnErrorReceived(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    CallbackOnSuccess(false, HttpStatusCode.Unauthorized);
                    break;
                default:
                    CallbackOnSuccess(false, statusCode);
                    break;
            }
        }

        public static void DeleteAllDataInDB()
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                db.RemoveRange(db.User);
                db.RemoveRange(db.Expense);
                db.RemoveRange(db.Group);
                db.RemoveRange(db.Picture);
                db.RemoveRange(db.Balance_User);
                db.RemoveRange(db.Debt_Expense);
                db.RemoveRange(db.Debt_Group);
                db.RemoveRange(db.Expense_Share);
                db.RemoveRange(db.Group_Members);
                db.SaveChanges();
            }
        }
    }
}
