using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using SplitBook.Views;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SplitBook.Add_Expense_Pages
{
    public sealed partial class EditExpense : Page
    {
        bool groupSelectionFirstTime = true;

        public EditExpense()
        {
            this.InitializeComponent();
            BackButton.Tapped += BackButton_Tapped;
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.expenseControl.Loaded += ExpenseControl_Loaded;
            this.expenseControl.groupListPicker.SelectionChanged += GroupListPicker_SelectionChanged;

            //setup the expenseShareUsers from the actual expense.
            this.expenseControl.expenseShareUsers.Clear();
            foreach (var item in this.expenseControl.expense.users)
            {
                this.expenseControl.expenseShareUsers.Add(item);
            }
            GoogleAnalytics.EasyTracker.GetTracker().SendView("EditExpensePage");
        }

        private void ExpenseControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetupViews();
        }

        private void SetupViews()
        {
            this.expenseControl.tbDescription.Text = this.expenseControl.expense.description;
            this.expenseControl.tbAmount.Text = this.expenseControl.expense.cost;

            if (!String.IsNullOrEmpty(this.expenseControl.expense.details) && !this.expenseControl.expense.details.Equals(Expense.DEFAULT_DETAILS))
            {
                this.expenseControl.tbDetails.Text = this.expenseControl.expense.details;
            }
            this.expenseControl.expenseDate.Date = DateTime.Parse(this.expenseControl.expense.date, System.Globalization.CultureInfo.InvariantCulture);
            this.expenseControl.groupList.SelectedItem = GetSelectedGroup();
            if (!String.IsNullOrEmpty(this.expenseControl.expense.receipt.large))
            {
                this.expenseControl.receiptImage.Source = new BitmapImage(new Uri(this.expenseControl.expense.receipt.large));
            }
            SetupSelectedUsers();

            //setup the payee
            int payeeCount = 0;
            Expense_Share payee = null;
            foreach (var item in this.expenseControl.expense.users)
            {
                if (item.hasPaid)
                {
                    payee = item;
                    payeeCount++;
                }
            }

            if (payeeCount > 1)
            {
                this.expenseControl.tbPaidBy.Text = "Multiple users";
                this.expenseControl.PaidByUser = null;
            }
            else
            {
                this.expenseControl.PaidByUser = payee;
                this.expenseControl.tbPaidBy.Text = payee.ToString();
            }

            //setup the expense to be split unequally for ease of operation
            this.expenseControl.SplitTypeListPicker.SelectedItem = AmountSplit.UnequalSplit;
        }

        private Group GetSelectedGroup()
        {
            //not assocated to any expense. therefore the groupSelectionChanged will not be fired.
            if (this.expenseControl.expense.group_id == 0)
            {
                groupSelectionFirstTime = false;
                return null;
            }
            else
            {
                foreach (var group in expenseControl.groupsList)
                {
                    if (this.expenseControl.expense.group_id == group.id)
                        return group;
                }
            }

            return null;
        }

        private void SetupSelectedUsers()
        {
            if (this.expenseControl.expense.users.Count == 0)
                return;
            else
            {
                foreach (var expenseUser in this.expenseControl.expense.users)
                {
                    //this.expenseControl.expenseShareUsers.Add(expenseUser);
                    if (expenseUser.user.id != App.currentUser.id)
                        this.expenseControl.friendList.SelectedItems.Add(expenseUser);
                }
            }
        }

        private async void BtnOk_Click(object sender, EventArgs e)
        {
            bool proceed = await this.expenseControl.SetupExpense();
            busyIndicator.IsActive = true;
            if (proceed)
                await EditExistingExpense();
            else
                busyIndicator.IsActive = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;
            this.Frame.GoBack();
        }

        //returns true only if one or less group is selected
        private bool ValidGroupSelected()
        {
            if (this.expenseControl.groupList.SelectedItems.Count > 1)
                return false;
            else
                return true;
        }

        private async void GroupListPicker_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //not to use the shortcut when we are filling up the details of the expense to be edited
            if (groupSelectionFirstTime)
            {
                groupSelectionFirstTime = false;
                return;
            }
            this.expenseControl.expense.group_id = 0;
            if (this.expenseControl.groupList.SelectedItem == null)
                return;

            if (ValidGroupSelected())
            {
                Group selectedGroup = this.expenseControl.groupList.SelectedItem as Group;
                this.expenseControl.expense.group_id = selectedGroup.id;

                //clear all the previoulsy selected friends.
                this.expenseControl.friendList.SelectedItems.Clear();

                foreach (var member in selectedGroup.members)
                {
                    //you don't need to add yourself as you will be added by default.
                    if (member.id == App.currentUser.id || this.expenseControl.friendList.SelectedItems.Contains(member))
                        continue;
                    this.expenseControl.friendList.SelectedItems.Add(new Expense_Share() { user = member, user_id = member.id });
                }
            }

            else
            {
                MessageDialog messageDialog = new MessageDialog("Sorry you can only select one group", "Error");
                await messageDialog.ShowAsync();
                //this.friendListPicker.SelectedItems.Clear();
                this.expenseControl.groupList.SelectedItems.Clear();
            }
        }

        private async Task EditExistingExpense()
        {
            ModifyDatabase modify = new ModifyDatabase(_editExpenseCompleted);
            await modify.EditExpense(this.expenseControl.expense);
        }

        private async void _editExpenseCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    (Application.Current as App).ADD_EXPENSE = null;
                    busyIndicator.IsActive = false;
                    this.Frame.Navigate(typeof(FriendsPage));
                    await MainPage.Current.FetchData();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.Logout();
                        (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Unable to edit expense", "Error");
                        await messageDialog.ShowAsync();
                    }
                });
            }
        }

        private async void BtnOkay_Click(object sender, RoutedEventArgs e)
        {
            bool proceed = await this.expenseControl.SetupExpense();
            busyIndicator.IsActive = true;
            if (proceed)
                await EditExistingExpense();
            else
                busyIndicator.IsActive = false;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;
            this.Frame.GoBack();
        }

        private async void BtnReceipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker open = new FileOpenPicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    ViewMode = PickerViewMode.Thumbnail
                };

                // Filter to include a sample subset of file types
                open.FileTypeFilter.Clear();
                open.FileTypeFilter.Add(".bmp");
                open.FileTypeFilter.Add(".png");
                open.FileTypeFilter.Add(".jpeg");
                open.FileTypeFilter.Add(".jpg");
                open.FileTypeFilter.Add(".pdf");

                // Open a stream for the selected file
                StorageFile file = await open.PickSingleFileAsync();

                // Ensure a file was selected
                if (file != null)
                {
                    // Ensure the stream is disposed once the image is loaded
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        if (file.FileType.Equals(".pdf"))
                        {
                            PdfDocument pdfDocument = await PdfDocument.LoadFromStreamAsync(fileStream);
                            using (PdfPage page = pdfDocument.GetPage(0))
                            {
                                var stream = new InMemoryRandomAccessStream();
                                await page.RenderToStreamAsync(stream);
                                await bitmapImage.SetSourceAsync(stream);
                            }
                        }
                        else
                        {
                            await bitmapImage.SetSourceAsync(fileStream);
                        }
                        this.expenseControl.receiptImage.Source = bitmapImage;
                    }
                    this.expenseControl.expense.receiptFile = file;
                }

            }
            catch (Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
            }
        }
    }
}
