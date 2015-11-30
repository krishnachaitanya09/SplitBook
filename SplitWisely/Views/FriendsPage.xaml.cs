using SplitWisely.Controller;
using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitWisely.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendsPage : Page
    {
        ObservableCollection<User> balanceFriends = new ObservableCollection<User>();
        ObservableCollection<User> youOweFriends = new ObservableCollection<User>();
        ObservableCollection<User> owesYouFriends = new ObservableCollection<User>();
        ObservableCollection<User> friendsList = new ObservableCollection<User>();
        ObservableCollection<Group> groupsList = new ObservableCollection<Group>();
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();

        private double postiveBalance = 0, negativeBalance = 0, totalBalance = 0;
        private NetBalances netBalanceObj = new NetBalances();

        BackgroundWorker syncDatabaseBackgroundWorker;
        SyncDatabase databaseSync;

        public FriendsPage()
        {
            this.InitializeComponent();

            llsFriends.ItemsSource = balanceFriends;

            populateData();

            syncDatabaseBackgroundWorker = new BackgroundWorker();
            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            else
            {
                bool firstUse = (bool)e.Parameter;
                databaseSync = new SyncDatabase(SyncConpleted);
                //busyIndicator.Content = "Syncing";
                //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
                //if the page has been accessed from the back stack
                if (firstUse)
                {
                    databaseSync.isFirstSync(true);
                    //busyIndicator.Content = "Setting up for first use";

                    //disable the add expense and searchtill first sycn is complete
                    //btnAddExpense.IsEnabled = false;
                    //btnSearchExpense.IsEnabled = false;
                }

                if (syncDatabaseBackgroundWorker.IsBusy != true)
                {
                    //busyIndicator.IsRunning = true;
                    syncDatabaseBackgroundWorker.RunWorkerAsync();
                    //btnRefresh.IsEnabled = false;
                }
            }
        }

        private void populateData()
        {
            loadFriends();
            //if (expenseLoadingBackgroundWorker.IsBusy != true)
            //{
            //    expenseLoadingBackgroundWorker.RunWorkerAsync();
            //}

            //if (groupLoadingBackgroundWorker.IsBusy != true)
            //{
            //    groupLoadingBackgroundWorker.RunWorkerAsync();
            //}
        }


        private void loadFriends()
        {
            friendsList.Clear();
            youOweFriends.Clear();
            owesYouFriends.Clear();
            balanceFriends.Clear();
            postiveBalance = 0;
            negativeBalance = 0;
            totalBalance = 0;

            QueryDatabase obj = new QueryDatabase();

            //only show balance below in the user's default currency
            foreach (var friend in obj.getAllFriends())
            {
                friendsList.Add(friend);
                Balance_User defaultBalance = Helpers.getDefaultBalance(friend.balance);
                double balance = System.Convert.ToDouble(defaultBalance.amount, CultureInfo.InvariantCulture);
                if (balance > 0)
                {
                    postiveBalance += balance;
                    totalBalance += balance;
                    owesYouFriends.Add(friend);
                    balanceFriends.Add(friend);
                }

                if (balance < 0)
                {
                    negativeBalance += balance;
                    totalBalance += balance;
                    youOweFriends.Add(friend);
                    balanceFriends.Add(friend);
                }
            }

            if (App.currentUser == null)
            {

                return;
            }

            //if default currency is not set then dont display the balances. Only the text is enough.
            if (App.currentUser.default_currency == null)
                return;
            netBalanceObj.setBalances(App.currentUser.default_currency, totalBalance, postiveBalance, negativeBalance);
            balancePanel.DataContext = netBalanceObj;
            //more.DataContext = App.currentUser;
        }

        private void syncDatabaseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            databaseSync.performSync();
        }

        private async void SyncConpleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //    busyIndicator.IsRunning = false;
                    //    pageNo = 0;
                    populateData();
                    //    hasDataLoaded = true;

                    //    btnAddExpense.IsEnabled = true;
                    //    btnSearchExpense.IsEnabled = true;

                    //    btnRefresh.IsEnabled = true;
                });
            }
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    //    btnRefresh.IsEnabled = true;

                    //    // don't need to handle the below two as there two are only disabled on first launch. If first launch sync fails, then these two buttons cannot be activated.
                    //    // btnAddExpense.IsEnabled = true;
                    //    //btnSearchExpense.IsEnabled = true;

                    //    busyIndicator.IsRunning = false;
                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.logout();
                        this.Frame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("Unable to sync with splitwise. You can continue to browse cached data", "Error");
                        await dialog.ShowAsync();
                    }
                });
            }
        }
    }
}
