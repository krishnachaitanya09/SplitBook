using SplitBook.Add_Expense_Pages;
using SplitBook.Controller;
using SplitBook.Controls;
using SplitBook.Converter.ExpandViewerConverters;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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

namespace SplitBook.Views
{
    public sealed partial class GroupDetailsPage : Page
    {
        Group selectedGroup;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        ObservableCollection<ExpandableListModel> expanderList = new ObservableCollection<ExpandableListModel>();
        ExpandableListModel currentUserExpanderInfo;
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public GroupDetailsPage()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;

            selectedGroup = (Application.Current as App).SELECTED_GROUP as Group;
            llsExpenses.ItemsSource = expensesList;

            SetupExpandableList();
            if (currentUserExpanderInfo != null && !currentUserExpanderInfo.isNonExpandable)
                settleBtn.IsEnabled = true;
            this.DataContext = selectedGroup;
            listBox.ItemsSource = expanderList;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Task.Run(async () =>
            {
                await LoadExpensesAsync();
            });            
            BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("GroupDetailsPage");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void SetupExpandableList()
        {
            foreach (var user in selectedGroup.members)
            {
                ExpandableListModel expanderItem = new ExpandableListModel()
                {
                    groupUser = user,
                    //if(selectedGroup.simplify_by_default)
                    debtList = Helpers.GetUsersGroupDebtsList(selectedGroup.simplified_debts, user.id)
                };
                //else
                // expanderItem.debtList = Util.getUsersGroupDebtsList(selectedGroup.original_debts, user.id);

                if (expanderItem.debtList.Count == 0)
                    expanderItem.isNonExpandable = true;

                expanderList.Add(expanderItem);

                if (user.id == App.currentUser.id)
                    currentUserExpanderInfo = expanderItem;
            }
        }

        private async Task LoadExpensesAsync()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.GetAllExpensesForGroup(selectedGroup.id, pageNo);
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
            if (sender is ListViewBase listview)
            {
                // Attach to the view changed event
                var _scrollViewer = listview.GetFirstDescendantOfType<ScrollViewer>();
                if (_scrollViewer != null)
                {
                    _scrollViewer.ViewChanged += OnViewChanged;
                }
            }
        }

        private async void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var _scrollViewer = sender as ScrollViewer;
            // If scrollviewer is scrolled down at least 80%
            if (_scrollViewer.VerticalOffset > Math.Max(_scrollViewer.ScrollableHeight * 0.8, _scrollViewer.ScrollableHeight - 200))
            {
                if (morePages)
                {
                    pageNo++;
                    await LoadExpensesAsync();
                }
            }
        }

        private void LlsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));

            llsExpenses.SelectedItem = null;
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            Expense expenseToAdd = new Expense()
            {
                group_id = selectedGroup.id
            };
            (Application.Current as App).ADD_EXPENSE = expenseToAdd;
            this.Frame.Navigate(typeof(AddExpense));
        }

        private void BtnSettle_Click(object sender, RoutedEventArgs e)
        {
            if (currentUserExpanderInfo.debtList.Count == 1)
                RecordPayment(currentUserExpanderInfo.debtList[0]);
            else
            {
                GroupSettleUpUserSelector ChoosePayeePopup = new GroupSettleUpUserSelector(currentUserExpanderInfo.debtList, SettleUpSelectorClose)
                {
                    MaxWidth = this.ActualWidth,
                    MinWidth = this.ActualWidth,
                    MaxHeight = Window.Current.Bounds.Height - 160
                };
                popup.Child = ChoosePayeePopup;
                popup.VerticalOffset = 40;
                popup.IsOpen = true;
            }
        }

        private void SettleUpSelectorClose(Debt_Group debt)
        {
            RecordPayment(debt);
        }

        private void RecordPayment(Debt_Group debt)
        {
            User user;
            String amount;
            if (debt.ownerId == debt.from)
            {
                (Application.Current as App).PAYMENT_TYPE = Constants.PAYMENT_TO;
                user = debt.toUser;

                //the amount for group debts is always in +ve but in payment page, we need it in correct +/- format
                amount = "-" + debt.amount;
            }
            else
            {
                (Application.Current as App).PAYMENT_TYPE = Constants.PAYMENT_FROM;
                user = debt.fromUser;
                amount = debt.amount;
            }
            if (user.balance == null)
                user.balance = new List<Balance_User>();

            user.balance.Add(new Balance_User() { amount = amount, currency_code = debt.currency_code, user_id = user.id });
            (Application.Current as App).PAYMENT_USER = user;
            (Application.Current as App).PAYMENT_GROUP = selectedGroup.id;

            this.Frame.Navigate(typeof(AddPayment));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
