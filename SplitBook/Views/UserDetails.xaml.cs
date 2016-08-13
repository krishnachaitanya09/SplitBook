using SplitBook.Add_Expense_Pages;
using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Email;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;
using System.Net;

namespace SplitBook.Views
{
    public sealed partial class UserDetails : Page
    {
        User selectedUser;
        BackgroundWorker userExpensesBackgroundWorker;
        BackgroundWorker deleteFriendBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public UserDetails()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            MainPage.Current.ResetNavMenu();


            deleteFriendBackgroundWorker = new BackgroundWorker();
            deleteFriendBackgroundWorker.WorkerSupportsCancellation = true;
            deleteFriendBackgroundWorker.DoWork += new DoWorkEventHandler(deleteFriendBackgroundWorker_DoWork);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            selectedUser = e.Parameter as User;

            llsExpenses.ItemsSource = expensesList;

            userExpensesBackgroundWorker = new BackgroundWorker();
            userExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            userExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(userExpensesBackgroundWorker_DoWork);

            if (userExpensesBackgroundWorker.IsBusy != true)
            {
                userExpensesBackgroundWorker.RunWorkerAsync();
            }

            if (hasOwesYouBalance())
                remind.IsEnabled = true;

            if (hasYouOweBalance() || hasOwesYouBalance())
                settle.IsEnabled = true;

            if (selectedUser.balance.Count == 0)
                selectedUser.balance.Add(new Balance_User() { amount = "0", currency_code = App.currentUser.default_currency, user_id = selectedUser.id });
            this.DataContext = selectedUser;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("UserDetailsPage");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void btnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            Expense expenseToAdd = new Expense();
            expenseToAdd.specificUserId = selectedUser.id;

            (Application.Current as App).ADD_EXPENSE = expenseToAdd;
            this.Frame.Navigate(typeof(AddExpense));
        }

        private async void btnDeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog("Are you sure?", "Delete Friend");
            messageDialog.Commands.Add(new UICommand { Label = "yes", Id = 0 });
            messageDialog.Commands.Add(new UICommand { Label = "no", Id = 1 });
            IUICommand result = await messageDialog.ShowAsync();

            switch ((int)result.Id)
            {
                case 0:
                    if (deleteFriendBackgroundWorker.IsBusy != true)
                    {
                        busyIndicator.IsActive = true;
                        deleteFriendBackgroundWorker.RunWorkerAsync();
                    }
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        private void btnSettle_Click(object sender, RoutedEventArgs e)
        {
            int navParams;
            if (hasOwesYouBalance())
                navParams = Constants.PAYMENT_FROM;
            else
                navParams = Constants.PAYMENT_TO;

            (Application.Current as App).PAYMENT_USER = selectedUser;
            (Application.Current as App).PAYMENT_TYPE = navParams;
            (Application.Current as App).PAYMENT_GROUP = 0;

            this.Frame.Navigate(typeof(AddPayment));           
        }

        private async void btnReminder_Click(object sender, RoutedEventArgs e)
        {
            string appUrl = "";
            string reminderText = "Hey there,\n\nThis is just a note to settle up on Splitwise as soon as you get the chance.\n\n";
            string thanks = "Thanks,\n";
            string userName = App.currentUser.first_name;
            string sentVia = "\n\nSent via,\n";
            string appName = "SplitBook! A splitwise client for Windows 10\n\n";
            string downloadApp = "Download app at: " + appUrl;

            EmailMessage emailComposeTask = new EmailMessage();
            emailComposeTask.Subject = "Settle up on Splitwise";
            emailComposeTask.Body = reminderText + thanks + userName + sentVia + appName;
            emailComposeTask.To.Add(new EmailRecipient(selectedUser.email));
            await EmailManager.ShowComposeNewEmailAsync(emailComposeTask);
        }


        private bool hasYouOweBalance()
        {
            foreach (var balance in selectedUser.balance)
            {
                if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) < 0)
                    return true;
            }
            return false;
        }

        private bool hasOwesYouBalance()
        {
            foreach (var balance in selectedUser.balance)
            {
                if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) > 0)
                    return true;
            }

            return false;
        }

        private void deleteFriendBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_deleteFriendCompleted);
            modify.deleteFriend(selectedUser.id);
        }

        private async void _deleteFriendCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (MainPage.syncDatabaseBackgroundWorker.IsBusy != true)
                    {
                        MainPage.syncDatabaseBackgroundWorker.RunWorkerAsync();
                    }
                    busyIndicator.IsActive = false;
                    this.Frame.Navigate(typeof(FriendsPage));
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;
                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.logout();
                        (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
                        MainPage.Current.Frame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Unable to delete friend", "Error");
                        await messageDialog.ShowAsync();
                    }
                });
            }
        }

        private void userExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private async void loadExpenses()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getExpensesForUser(selectedUser.id, pageNo);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;

                foreach (var expense in allExpenses)
                {
                    expensesList.Add(expense);
                }
            });
        }

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            var listview = sender as ListViewBase;
            if (listview != null)
            {
                // Attach to the view changed event
                var _scrollViewer = listview.GetFirstDescendantOfType<ScrollViewer>();
                if (_scrollViewer != null)
                {
                    _scrollViewer.ViewChanged += OnViewChanged;
                }
            }
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var _scrollViewer = sender as ScrollViewer;
            // If scrollviewer is scrolled down at least 90%
            if (_scrollViewer.VerticalOffset > Math.Max(_scrollViewer.ScrollableHeight * 0.8, _scrollViewer.ScrollableHeight - 200))
            {
                if (userExpensesBackgroundWorker.IsBusy != true && morePages)
                {
                    pageNo++;
                    userExpensesBackgroundWorker.RunWorkerAsync(false);
                }
            }
        }

        private void ProfilePic_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as BitmapImage;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic = pic;
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));
            llsExpenses.SelectedItem = null;
        }
    }
}
