using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace SplitBook.Utilities
{
    public class Helpers
    {
        public async void CreateDatabase()
        {
            try
            {
                if (!await CheckFileExists(Constants.DATABASE_NAME))
                {
                    using (SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, true))
                    {
                        dbConn.CreateTable<User>();
                        dbConn.CreateTable<Expense>();
                        dbConn.CreateTable<Model.Group>();
                        dbConn.CreateTable<Picture>();
                        dbConn.CreateTable<Receipt>();
                        dbConn.CreateTable<Balance_User>();
                        dbConn.CreateTable<Debt_Expense>();
                        dbConn.CreateTable<Debt_Group>();
                        dbConn.CreateTable<Expense_Share>();
                        dbConn.CreateTable<Group_Members>();
                        dbConn.CreateTable<Currency>();
                        dbConn.CreateTable<Notifications>();
                    }
                }
            }
            catch (Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
            }
        }

        public async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetAppVersion()
        {

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

        }

        public static string GetQueryParameter(string input, string parameterName)
        {
            foreach (string item in input.Split('&'))
            {
                var parts = item.Split('=');
                if (parts[0] == parameterName)
                {
                    return parts[1];
                }
            }
            return String.Empty;
        }

        public static string AccessToken
        {
            get { return (string)ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_TAG]; }
            set { ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_TAG] = value; }
        }

        public static string AccessTokenSecret
        {
            get { return (string)ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_SECRET_TAG]; }
            set { ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_SECRET_TAG] = value; }
        }

        public static string AppVersion
        {
            get { return (string)ApplicationData.Current.LocalSettings.Values[Constants.APP_VERSION]; }
            set { ApplicationData.Current.LocalSettings.Values[Constants.APP_VERSION] = value; }
        }

        public static void SetLastUpdatedTime()
        {
            DateTime now = DateTime.UtcNow;
            string lastUpdatedTime = now.ToString("yyyy-MM-ddTHH:mm:ssK");
            ApplicationData.Current.LocalSettings.Values[Constants.LAST_UPDATED_TIME] = lastUpdatedTime;
        }

        public static string GetLastUpdatedTime()
        {
            return (string)ApplicationData.Current.LocalSettings.Values[Constants.LAST_UPDATED_TIME] ?? "0";
        }

        public static string NotificationsLastUpdated
        {
            get { return (string)ApplicationData.Current.LocalSettings.Values["notifications_last_updated"]; }
            set { ApplicationData.Current.LocalSettings.Values["notifications_last_updated"] = value; }
        }

        public static void SetCurrentUserId(int userId)
        {
            ApplicationData.Current.LocalSettings.Values[Constants.CURRENT_USER_ID] = userId;
        }

        public static int GetCurrentUserId()
        {
            return Convert.ToInt32(ApplicationData.Current.LocalSettings.Values[Constants.CURRENT_USER_ID]);
        }

        public static void SetDonNotShowDebtSimplifationBox()
        {
            ApplicationData.Current.LocalSettings.Values[Constants.DEBT_SIMPLIFICATION_DO_NOT_SHOW] = true;
        }

        public static bool DoNotShowDebtSimplificationBox()
        {
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[Constants.DEBT_SIMPLIFICATION_DO_NOT_SHOW]);
        }

        public static bool CheckNetworkConnection()
        {
            bool IsConnected = false;
            if (NetworkInterface.GetIsNetworkAvailable())
                IsConnected = true;
            else
                IsConnected = false;
            return IsConnected;
        }

        public static Balance_User GetDefaultBalance(List<Balance_User> balance)
        {
            try
            {
                //Each balance entry represents a balance in a seperate currency
                string currency = App.currentUser.default_currency;

                if (balance == null || balance.Count == 0)
                {
                    Balance_User noBalance = new Balance_User()
                    {
                        amount = "0"
                    };
                    return noBalance;
                }

                if (currency == null)
                {
                    return balance[0];
                }

                foreach (var userBalance in balance)
                {
                    if (userBalance.currency_code.Equals(currency))
                    {
                        return userBalance;
                    }
                }
                return balance[0];
            }
            catch
            {
                return balance[0];
            }
        }

        public static bool HasMultipleBalances(List<Balance_User> balance)
        {
            if (balance.Count > 1)
                return true;
            else
                return false;
        }

        public static List<Debt_Group> GetUsersGroupDebtsList(List<Debt_Group> allDebts, int userId)
        {
            List<Debt_Group> currentUserDebts = new List<Debt_Group>();
            for (int i = 0; i < allDebts.Count; i++)
            {
                Debt_Group debtGroup = new Debt_Group(allDebts[i]);
                if (debtGroup.from == userId || debtGroup.to == userId)
                {
                    debtGroup.ownerId = userId;
                    currentUserDebts.Add(debtGroup);
                }
            }
            return currentUserDebts;
        }

        public static double GetUserGroupDebtAmount(List<Debt_Group> allDebts, int userId)
        {
            double amount = 0;
            List<Debt_Group> currentUserDebts = new List<Debt_Group>();
            currentUserDebts = GetUsersGroupDebtsList(allDebts, userId);
            foreach (var debt in currentUserDebts)
            {
                if (debt.from == userId)
                    amount -= Convert.ToDouble(debt.amount, CultureInfo.InvariantCulture);
                else
                    amount += Convert.ToDouble(debt.amount, CultureInfo.InvariantCulture);
            }

            return amount;
        }

        public static void Logout()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();
            App.currentUser = null;
            SyncDatabase.DeleteAllDataInDB();
        }

        public static bool IsValidEmail(string emailAddress)
        {
            // Return true if emailAddress is in valid e-mail format.
            return Regex.IsMatch(emailAddress, @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }

        public static bool IsEmpty(string value)
        {
            if (value == null)
                return true;

            value = value.Trim();
            if (value.Length == 0)
                return true;

            return false;
        }
    }
}
