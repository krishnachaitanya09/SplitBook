using SplitWisely.Model;
using SplitWisely.Request;
using SplitWisely.Utilities;
using SQLite;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Controller
{
    public class ModifyDatabase
    {
        Action<bool, HttpStatusCode> callback;

        public ModifyDatabase(Action<bool, HttpStatusCode> callback)
        {
            this.callback = callback;
        }

        public void deleteExpense(int expenseId)
        {
            DeleteExpenseRequest request = new DeleteExpenseRequest(expenseId);
            request.deleteExpense(_OperationSucceded, _OperationFailed);
        }

        public void editExpense(Expense editedExpenseDetail)
        {
            UpdateExpenseRequest request = new UpdateExpenseRequest(editedExpenseDetail);
            request.updateExpense(_OperationSucceded, _OperationFailed);
        }

        public void addExpense(Expense expense)
        {
            AddExpenseRequest request = new AddExpenseRequest(expense);
            request.addExpense(_OperationSucceded, _OperationFailed);
        }

        public void createFriend(string email, string firstName, string lastName)
        {
            CreateFriendRequest request = new CreateFriendRequest(email, firstName, lastName);
            request.createFriend(_FriendAdded, _OperationFailed);
        }

        public void createGroup(Group group)
        {
            CreateGroupRequest request = new CreateGroupRequest(group);
            request.createGroup(_GroupAdded, _OperationFailed);
        }

        private void _FriendAdded(User friend)
        {
            //add user to database and to friends list in App.xaml
            //PhoneApplicationService.Current.State[Constants.NEW_USER] = friend;

            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, true))
            {
                dbConn.Insert(friend);
                friend.picture.user_id = friend.id;
                dbConn.Insert(friend.picture);
            }

            callback(true, HttpStatusCode.OK);
        }

        private void _GroupAdded(Group group)
        {
            //add user to database and to friends list in App.xaml
            //PhoneApplicationService.Current.State[Constants.NEW_GROUP] = group;

            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Constants.DB_PATH, true))
            {
                dbConn.BeginTransaction();
                dbConn.Insert(group);

                foreach (var debt in group.simplified_debts)
                {
                    debt.group_id = group.id;
                    dbConn.InsertOrReplace(debt);
                }

                foreach (var member in group.members)
                {
                    Group_Members group_member = new Group_Members();
                    group_member.group_id = group.id;
                    group_member.user_id = member.id;
                    dbConn.InsertOrReplace(group_member);
                }

                dbConn.Commit();
            }

            callback(true, HttpStatusCode.OK);
        }
        
        
        private void _OperationSucceded(bool status)
        {
            callback(true, HttpStatusCode.OK);
        }

        private void _OperationFailed(HttpStatusCode statusCode)
        {
            callback(false, statusCode);
        }
    }
}
