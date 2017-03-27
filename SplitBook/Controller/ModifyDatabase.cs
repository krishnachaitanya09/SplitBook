using SplitBook.Model;
using SplitBook.Request;
using SplitBook.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Controller
{
    public class ModifyDatabase
    {
        Action<bool, HttpStatusCode> callback;

        public ModifyDatabase(Action<bool, HttpStatusCode> callback)
        {
            this.callback = callback;
        }

        public async Task DeleteExpense(int expenseId)
        {
            DeleteExpenseRequest request = new DeleteExpenseRequest(expenseId);
            await request.DeleteExpense(_OperationSucceded, _OperationFailed);
        }

        public async Task EditExpense(Expense editedExpenseDetail)
        {
            UpdateExpenseRequest request = new UpdateExpenseRequest(editedExpenseDetail);
            await request.UpdateExpense(_OperationSucceded, _OperationFailed);
        }

        public async Task AddExpense(Expense expense)
        {
            AddExpenseRequest request = new AddExpenseRequest(expense);
            await request.AddExpense(_OperationSucceded, _OperationFailed);
        }

        public async Task CreateFriend(string email, string firstName, string lastName)
        {
            CreateFriendRequest request = new CreateFriendRequest(email, firstName, lastName);
            await request.CreateFriend(_FriendAdded, _OperationFailed);
        }

        public async Task DeleteFriend(int friendId)
        {
            DeleteFriendRequest request = new DeleteFriendRequest(friendId);
            await request.DeleteFriend(_OperationSucceded, _OperationFailed);
        }

        public async Task CreateGroup(Group group)
        {
            CreateGroupRequest request = new CreateGroupRequest(group);
            await request.CreateGroup(_GroupAdded, _OperationFailed);
        }

        private void _FriendAdded(User friend)
        {
            //add user to database and to friends list in App.xaml
            //PhoneApplicationService.Current.State[Constants.NEW_USER] = friend;

            using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, true))
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

            using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, true))
            {
                dbConn.BeginTransaction();
                dbConn.InsertOrReplace(group);

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
