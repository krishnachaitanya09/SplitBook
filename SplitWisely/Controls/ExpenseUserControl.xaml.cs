using SplitWisely.Controller;
using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;


namespace SplitWisely.Controls
{
    public sealed partial class ExpenseUserControl : UserControl
    {
        public Expense expense;

        private BackgroundWorker getSupportedCurrenciesBackgroundWorker;

        public ObservableCollection<Expense_Share> friends = new ObservableCollection<Expense_Share>();
        public ObservableCollection<Group> groupsList = new ObservableCollection<Group>();

        private ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        public ObservableCollection<Expense_Share> expenseShareUsers = new ObservableCollection<Expense_Share>();

        //this will ONLY be null if there are multiple paid by users.
        public Expense_Share PaidByUser;

        //By default the amount is split equally
        public AmountSplit amountSplit = AmountSplit.EqualSplit;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public ExpenseUserControl()
        {
            this.InitializeComponent();
            friendListPicker.AddHandler(TappedEvent, new TappedEventHandler(friendListPicker_Tapped), true);
            groupListPicker.AddHandler(TappedEvent, new TappedEventHandler(groupListPicker_Tapped), true);

            loadFriendsAndGroups();

            if ((Application.Current as App).ADD_EXPENSE != null)
                expense = (Application.Current as App).ADD_EXPENSE;

            getSupportedCurrenciesBackgroundWorker = new BackgroundWorker();
            getSupportedCurrenciesBackgroundWorker.WorkerSupportsCancellation = true;
            getSupportedCurrenciesBackgroundWorker.DoWork += new DoWorkEventHandler(getSupportedCurrenciesBackgroundWorker_DoWork);

            if (!getSupportedCurrenciesBackgroundWorker.IsBusy)
                getSupportedCurrenciesBackgroundWorker.RunWorkerAsync();

            this.friendList.ItemsSource = friends;

            this.groupList.ItemsSource = groupsList;

            if (groupsList == null || groupsList.Count == 0)
                this.groupListPicker.Visibility = Visibility.Collapsed;

            this.currencyListPicker.ItemsSource = currenciesList;

            this.SplitTypeListPicker.ItemsSource = AmountSplit.GetAmountSplitTypes();
            this.SplitTypeListPicker.SelectedIndex = 0;

            if (expense == null)
            {
                expense = new Expense();
                expense.group_id = 0; //by default the expense is in no group
            }

            //add current user
            expenseShareUsers.Add(getCurrentUser());
            PaidByUser = getCurrentUser();
            tbPaidBy.Text = PaidByUser.ToString();
        }

        //public void SetupListeners()
        //{
        //    this.friendList.SelectionChanged += FriendListPicker_SelectionChanged;
        //    this.groupList.SelectionChanged += GroupList_SelectionChanged;
        //    this.SplitTypeListPicker.SelectionChanged += SplitTypeListPicker_SelectionChanged;
        //    this.expenseTypeListPicker.SelectionChanged += expenseTypeListPicker_SelectionChanged;
        //}

        private Expense_Share getCurrentUser()
        {
            return new Expense_Share() { user = App.currentUser, user_id = App.currentUser.id };
        }

        private void loadFriendsAndGroups()
        {
            loadGroups();
            loadFriends();
        }

        private void loadGroups()
        {
            QueryDatabase obj = new QueryDatabase();
            List<Group> allGroups = obj.getAllGroups();

            if (allGroups != null && allGroups.Count != 0)
            {
                this.groupListPicker.Visibility = Visibility.Visible;
                foreach (var group in allGroups)
                {
                    groupsList.Add(group);
                }
            }
        }

        private void loadFriends()
        {
            QueryDatabase obj = new QueryDatabase();
            List<User> allFriends = obj.getAllFriends();
            if (allFriends != null)
            {
                foreach (var friend in allFriends)
                {
                    friends.Add(new Expense_Share() { user = friend, user_id = friend.id });
                }
            }
        }

        #region Event Handlers

        private string FriendSummary()
        {
            string summary = String.Empty;
            for (int i = 0; i < friendList.SelectedItems.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == friendList.SelectedItems.Count - 1;

                Expense_Share friend = (Expense_Share)friendList.SelectedItems[i];
                summary = String.Concat(summary, friend.user.first_name);
                summary += isLast ? string.Empty : ", ";
            }
            if (summary == String.Empty)
            {
                summary = "no friends selected";
            }
            return summary;
        }

        private string GroupSummary()
        {
            string summary = String.Empty;
            for (int i = 0; i < groupList.SelectedItems.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == groupList.SelectedItems.Count - 1;

                Group group = (Group)groupList.SelectedItems[i];
                summary = String.Concat(summary, group.name);
                summary += isLast ? string.Empty : ", ";
            }
            if (summary == String.Empty)
            {
                summary = "select all members of group";
            }
            return summary;
        }

        private async void getSupportedCurrenciesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.getSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency && String.IsNullOrEmpty(expense.currency_code))
                    defaultCurrency = item;

                else if (!String.IsNullOrEmpty(expense.currency_code))
                {
                    if (item.currency_code == expense.currency_code)
                        defaultCurrency = item;
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    currenciesList.Add(item);
                });
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.currencyListPicker.SelectedItem = defaultCurrency;
            });
        }

        private void friendListPicker_Tapped(object sender, TappedRoutedEventArgs e)
        {

            ShowPopup(ref friendListPopup);
        }

        private void groupListPicker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ShowPopup(ref groupListPopup);
        }

        private async void tbPaidBy_Tap(object sender, TappedRoutedEventArgs e)
        {
            if (expenseShareUsers.Count <= 1)
                return;

            //need can proceed as user can select multiple payee and we need cost for that.
            if (await canProceed())
            {
                SelectPayeePopUpControl ChoosePayeePopup = new SelectPayeePopUpControl(expenseShareUsers, _PayeeClose);
                ChoosePayeePopup.MaxWidth = this.ActualWidth;
                ChoosePayeePopup.MinWidth = this.ActualWidth;
                ChoosePayeePopup.MaxHeight = this.ActualHeight / 1.3;
                contentDialog.Content = ChoosePayeePopup;
                await contentDialog.ShowAsync();
            }
        }

        /**
         * If isMultiplePayer is true then SelectedUser can be null
         */
        private void _PayeeClose(Expense_Share SelectedUser, bool isMultiplePayer)
        {
            contentDialog.Hide();

            //Set the paid amount of everyone to 0 and the selected user's hasPaid to true.
            //The selected user's cost is set during the setupExpense method.
            if (isMultiplePayer)
            {
                //Show the multiple payer popup
                showMultiplePayeePopUp();
            }
            else
            {
                for (int i = 0; i < expenseShareUsers.Count; i++)
                {
                    expenseShareUsers[i].paid_share = "0.0";

                    if (expenseShareUsers[i].Equals(SelectedUser))
                        expenseShareUsers[i].hasPaid = true;
                    else
                        expenseShareUsers[i].hasPaid = false;
                }
                PaidByUser = SelectedUser;
                tbPaidBy.Text = PaidByUser.ToString();
            }
        }

        private async void showMultiplePayeePopUp()
        {
            String cost;
            if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                cost = tbAmount.Text.Replace(".", ",");
            else
                cost = tbAmount.Text.Replace(",", ".");
            MultiplePayeeInputPopUpControl MultiplePayeeInputPopup = new MultiplePayeeInputPopUpControl
                                                            (ref expenseShareUsers, _MultiplePayeeInputClose, Convert.ToDecimal(cost));
            MultiplePayeeInputPopup.MaxWidth = this.ActualWidth / 1.2;
            MultiplePayeeInputPopup.MinWidth = this.ActualWidth / 1.2;
            MultiplePayeeInputPopup.MaxHeight = this.ActualHeight / 1.3;
            MultiplePayeeDialog.Content = MultiplePayeeInputPopup;
            await MultiplePayeeDialog.ShowAsync();
        }

        //This is called after validation has been done and okay has been pressed.
        private void _MultiplePayeeInputClose()
        {
            MultiplePayeeDialog.Hide();
            PaidByUser = null;
            tbPaidBy.Text = "Multiple users";
        }

        async void SplitTypeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountSplit = (AmountSplit)this.SplitTypeListPicker.SelectedItem;
            if (amountSplit.id == AmountSplit.TYPE_SPLIT_UNEQUALLY)
            {
                if (await canProceed())
                {
                    //show unequall split pop up;
                    String cost;
                    if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                        cost = tbAmount.Text.Replace(".", ",");
                    else
                        cost = tbAmount.Text.Replace(",", "."); ;
                    UnequallySplit UnequallySplitPopup = new UnequallySplit(expenseShareUsers, Convert.ToDecimal(cost), _UnequallyClose);
                    UnequallySplitPopup.MaxHeight = this.ActualHeight / 1.3;
                    UnequallySplitPopup.MaxWidth = this.ActualWidth / 1.15;
                    UnequallySplitPopup.MinWidth = this.ActualWidth / 1.15;

                    contentDialog.Content = UnequallySplitPopup;
                    await contentDialog.ShowAsync();
                }
                else
                    SplitTypeListPicker.SelectedItem = AmountSplit.EqualSplit;
            }
        }

        private void _UnequallyClose(ObservableCollection<Expense_Share> users)
        {
            contentDialog.Hide();
            this.expenseShareUsers = users;
        }

        private void ShowPopup(ref Popup popup)
        {
            popup.IsOpen = true;
        }


        void FriendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.friendListPicker.Text = this.FriendSummary();

            int count = friendList.SelectedItems.Count;

            if (count == 1)
            {
                Expense_Share selectedFriend = (Expense_Share)friendList.SelectedItem;
                this.expenseTypeListPicker.ItemsSource = ExpenseType.GetAllExpenseTypeList(selectedFriend.user.first_name);
                this.expenseTypeListPicker.SelectedIndex = 0;
                this.expenseTypeListPicker.Visibility = Visibility.Visible;
            }
            else
            {
                this.expenseTypeListPicker.ItemsSource = ExpenseType.GetOnlySplitExpenseTypeList();
                this.expenseTypeListPicker.SelectedIndex = 0;
                this.expenseTypeListPicker.Visibility = Visibility.Collapsed;
            }

            foreach (var item in e.AddedItems)
            {
                if (expenseShareUsers.Contains(item))
                    return;
                expenseShareUsers.Add(item as Expense_Share);
            }

            foreach (var item in e.RemovedItems)
            {
                expenseShareUsers.Remove(item as Expense_Share);
                if (item == PaidByUser)
                {
                    PaidByUser = getCurrentUser();
                    tbPaidBy.Text = PaidByUser.ToString();
                }
            }
        }

        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.groupListPicker.Text = this.GroupSummary();
        }


        void expenseTypeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.expenseTypeListPicker.SelectedItem != null)
            {
                ExpenseType type = (ExpenseType)this.expenseTypeListPicker.SelectedItem;
                if (type.id != ExpenseType.TYPE_SPLIT_BILL)
                    this.paidByContainer.Visibility = Visibility.Collapsed;
                else
                    this.paidByContainer.Visibility = Visibility.Visible;
            }
        }

        private async Task<bool> canProceed()
        {
            if (String.IsNullOrEmpty(tbAmount.Text) || friendList.SelectedItems == null || friendList.SelectedItems.Count == 0)
            {
                var dialog = new MessageDialog("Please select expense friends and enter the expense amount.")
                {
                    Title = "Error"
                };
                await dialog.ShowAsync();
                return false;
            }
            return true;
        }
        #endregion

        public async Task<bool> setupExpense()
        {
            try
            {
                FocusedTextBoxUpdateSource();
                //to hide the keyboard if any
                this.Focus(FocusState.Programmatic);
                bool proceed = true;

                //Check if description and amount are present.
                expense.description = tbDescription.Text;

                if (!Helpers.isEmpty(tbAmount.Text))
                {
                    if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                        expense.cost = tbAmount.Text.Replace(".", ",");
                    else
                        expense.cost = tbAmount.Text.Replace(",", ".");
                }

                if (String.IsNullOrEmpty(expense.cost) || String.IsNullOrEmpty(expense.description))
                {
                    var dialog = new MessageDialog("Please enter amount and description")
                    {
                        Title = "Error"
                    };
                    await dialog.ShowAsync();
                    return false;
                }

                if (friendList.SelectedItems == null || friendList.SelectedItems.Count == 0)
                {
                    var dialog = new MessageDialog("Please enter amount and description")
                    {
                        Title = "Error"
                    };
                    await dialog.ShowAsync();
                    return false;
                }

                ExpenseType type = (ExpenseType)this.expenseTypeListPicker.SelectedItem;
                //Type is null when expenseTypeListPicker is not visibile, i.e. more than 1 expense user is selected.
                if (type == null)
                    proceed = await SplitBillType();
                else
                {
                    switch (type.id)
                    {
                        case ExpenseType.TYPE_FRIEND_OWES:
                            PaidByUser = getCurrentUser();
                            fullExpenseSplit();
                            break;
                        case ExpenseType.TYPE_YOU_OWE:
                            PaidByUser = friendList.SelectedItem as Expense_Share;
                            fullExpenseSplit();
                            break;
                        case ExpenseType.TYPE_SPLIT_BILL:
                            proceed = await SplitBillType();
                            break;
                        default:
                            break;
                    }
                }

                expense.users = expenseShareUsers.ToList();
                Currency selectedCurrency = (currencyListPicker.SelectedItem as Currency);

                if (selectedCurrency != null)
                    expense.currency_code = selectedCurrency.currency_code;

                expense.payment = false;
                expense.details = tbDetails.Text;
                DateTime dateTime = expenseDate.Date.Date;
                expense.date = dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");

                return proceed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SplitBillType()
        {
            bool proceed = true;
            //This can have split equally or unequally;
            switch (amountSplit.id)
            {
                case AmountSplit.TYPE_SPLIT_EQUALLY:
                    proceed = await divideExpenseEqually();
                    break;
                case AmountSplit.TYPE_SPLIT_UNEQUALLY:
                    proceed = await divideExpenseUnequally();
                    break;
                default:
                    break;
            }

            return proceed;
        }

        private async Task<bool> divideExpenseEqually()
        {
            double totalPaidBy = 0;

            int numberOfExpenseMembers = expenseShareUsers.Count;
            double amountToSplit = Convert.ToDouble(expense.cost);
            double perPersonShare = amountToSplit / numberOfExpenseMembers;
            perPersonShare = Math.Round(perPersonShare, 2, MidpointRounding.AwayFromZero);

            //now make sure that the total is the same, else take care of the difference (should be very minimal)
            double currentUsersShare = perPersonShare;
            double total = perPersonShare * numberOfExpenseMembers;
            if (total == amountToSplit)
                currentUsersShare = perPersonShare;
            else
                currentUsersShare = currentUsersShare + (amountToSplit - total);

            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                if (PaidByUser != null && expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = amountToSplit.ToString();
                }

                if (expenseShareUsers[i].user_id == App.currentUser.id)
                    expenseShareUsers[i].owed_share = currentUsersShare.ToString();
                else
                {
                    //do not set paid_share as 0 as it might override the value set by multiple payers.
                    //expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = perPersonShare.ToString();
                }
                if (!String.IsNullOrEmpty(expenseShareUsers[i].paid_share))
                    totalPaidBy += Convert.ToDouble(expenseShareUsers[i].paid_share);

                expenseShareUsers[i].share = "1";
            }

            if (totalPaidBy == amountToSplit)
                return true;
            else
            {
                var dialog = new MessageDialog("The \"Paid by\" does not add upto the total expense cost.")
                {
                    Title = "Error"
                };
                await dialog.ShowAsync();
                return false;
            }
        }

        //The unequallySplit popup has already taken care of owed_share. here we only need to handle the paidshare
        //the paid share is handled with the help of PaidByUser. If this is null, then it means we have a Multiple Payee scenario
        //and in that case, even the paid share is already handled for us by the MultiplePayeeInputPopUpControl
        private async Task<bool> divideExpenseUnequally()
        {
            decimal totalPaidBy = 0;
            decimal totalOwed = 0;
            decimal amountToSplit = Convert.ToDecimal(expense.cost);
            int numberOfExpenseMembers = expenseShareUsers.Count;

            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                if (PaidByUser != null && expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = amountToSplit.ToString();
                }

                if (!String.IsNullOrEmpty(expenseShareUsers[i].paid_share))
                    totalPaidBy += Convert.ToDecimal(expenseShareUsers[i].paid_share);

                if (!String.IsNullOrEmpty(expenseShareUsers[i].owed_share))
                    totalOwed += Convert.ToDecimal(expenseShareUsers[i].owed_share);
            }

            if (totalPaidBy == amountToSplit && totalOwed == amountToSplit)
                return true;
            else
            {
                if (totalPaidBy != amountToSplit)
                {
                    var dialog = new MessageDialog("The \"Paid by\" does not add upto the total expense cost.")
                    {
                        Title = "Error"
                    };
                    await dialog.ShowAsync();
                }
                if (totalOwed != amountToSplit)
                {
                    var dialog = new MessageDialog("The \"Paid by\" does not add upto the total expense cost.")
                    {
                        Title = "Error"
                    };
                    await dialog.ShowAsync();
                }
                return false;
            }
        }

        private void fullExpenseSplit()
        {
            //only you and one more user should be there to access this feature
            if (expenseShareUsers.Count != 2)
                throw new IndexOutOfRangeException();
            int numberOfExpenseMembers = expenseShareUsers.Count;
            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                if (expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = expense.cost;
                    expenseShareUsers[i].owed_share = "0";
                }
                else
                {
                    expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = expense.cost;
                }
            }
        }

        public static void FocusedTextBoxUpdateSource()
        {
            var focusedElement = FocusManager.GetFocusedElement();
            var focusedTextBox = focusedElement as TextBox;

            if (focusedTextBox != null)
            {
                var binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);

                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                var focusedPasswordBox = focusedElement as PasswordBox;

                if (focusedPasswordBox != null)
                {
                    var binding = focusedPasswordBox.GetBindingExpression(PasswordBox.PasswordProperty);

                    if (binding != null)
                    {
                        binding.UpdateSource();
                    }
                }
            }
        }

        private void friendListPicker_Loaded(object sender, RoutedEventArgs e)
        {
            friendListPicker.Text = FriendSummary();
        }

        private void groupListPicker_Loaded(object sender, RoutedEventArgs e)
        {
            groupListPicker.Text = GroupSummary();
        }
    }
}
