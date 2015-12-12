using SplitWisely.Controller;
using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitWisely.Views
{
    public sealed partial class UserDetails : Page
    {
        User selectedUser;
        BackgroundWorker userExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public UserDetails()
        {
            this.InitializeComponent();
            MainPage.Current.ResetNavMenu();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selectedUser = e.Parameter as User;

            llsExpenses.ItemsSource = expensesList;

            userExpensesBackgroundWorker = new BackgroundWorker();
            userExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            userExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(userExpensesBackgroundWorker_DoWork);

            if (userExpensesBackgroundWorker.IsBusy != true)
            {
                userExpensesBackgroundWorker.RunWorkerAsync();
            }
            this.DataContext = selectedUser;
            if (selectedUser.balance.Count == 0)
                selectedUser.balance.Add(new Balance_User() { amount = "0", currency_code = App.currentUser.default_currency, user_id = selectedUser.id });
            this.balanceList.ItemsSource = selectedUser.balance;
        }

        private void btnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            Expense expenseToAdd = new Expense();
            expenseToAdd.specificUserId = selectedUser.id;

            //PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = expenseToAdd;
            //NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddExpense.xaml", UriKind.Relative));
        }

        private void btnSettle_Click(object sender, RoutedEventArgs e)
        {
            int navParams;
            if (hasOwesYouBalance())
                navParams = Constants.PAYMENT_FROM;
            else
                navParams = Constants.PAYMENT_TO;

            //PhoneApplicationService.Current.State[Constants.PAYMENT_USER] = selectedUser;
            //PhoneApplicationService.Current.State[Constants.PAYMENT_TYPE] = navParams;
            //PhoneApplicationService.Current.State[Constants.PAYMENT_GROUP] = 0;

            //NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddPayment.xaml", UriKind.Relative));
        }

        private async void btnReminder_Click(object sender, RoutedEventArgs e)
        {
            string appUrl = "";
            string reminderText = "Hey there,\n\nThis is just a note to settle up on Splitwise as soon as you get the chance.\n\n";
            string thanks = "Thanks,\n";
            string userName = App.currentUser.first_name;
            string sentVia = "\n\nSent via,\n";
            string appName = "Split it! A splitwise client for windows phone\n\n";
            string downloadApp = "Download app at: " + appUrl;

            EmailMessage emailComposeTask = new EmailMessage();
            emailComposeTask.Subject = "Settle up on Splitwise";
            emailComposeTask.Body = reminderText + thanks + userName + sentVia + appName + downloadApp;
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

        private void userExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Task.Run(async () =>
            {
                await loadExpenses();
            });
        }

        private async Task loadExpenses()
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
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            //PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            //NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));

            llsExpenses.SelectedItem = null;
        }
    }
}
