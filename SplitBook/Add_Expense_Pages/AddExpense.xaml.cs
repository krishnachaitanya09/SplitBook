using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using SplitBook.Views;
using System;
using System.ComponentModel;
using System.IO;
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
    public sealed partial class AddExpense : Page
    {
        public AddExpense()
        {
            this.InitializeComponent();
            BackButton.Tapped += BackButton_Tapped;

            this.expenseControl.amountSplit = AmountSplit.EqualSplit;
            this.expenseControl.groupList.SelectionChanged += GroupListPicker_SelectionChanged;

            //This helps to auto-populate if the user is coming from the GroupDetails or UserDetails page
            AutoPopulateGroup();
            AutoPopulateExpenseShareUsers();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("AddExpensePage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void AutoPopulateGroup()
        {
            if (this.expenseControl.expense == null)
                return;
            else
            {
                foreach (var group in expenseControl.groupsList)
                {
                    if (this.expenseControl.expense.group_id == group.id)
                        this.expenseControl.groupList.SelectedItem = group;
                }
            }
        }

        private void AutoPopulateExpenseShareUsers()
        {
            if (this.expenseControl.expense == null)
                return;
            else
            {
                foreach (var user in expenseControl.friends)
                {
                    if (this.expenseControl.expense.specificUserId == user.user_id)
                        this.expenseControl.friendList.SelectedItems.Add(user);
                }
            }
        }

        private async void BtnOkay_Click(object sender, RoutedEventArgs e)
        {
            commandBar.IsEnabled = false;
            bool proceed = await this.expenseControl.SetupExpense();
            busyIndicator.IsActive = true;
            if (proceed)
                await AddNewExpense();
            else
            {
                busyIndicator.IsActive = false;
                commandBar.IsEnabled = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;

            //This page might be accessed from the start screen tile and hence canGoBack might be false
            if (this.Frame.CanGoBack)
                this.Frame.Navigate(typeof(FriendsPage));
            else
                Application.Current.Exit();
        }

        //private void btnPin_Click(object sender, EventArgs e)
        //{
        //    StandardTileData tileData = new StandardTileData
        //    {
        //        Title = "add expense",
        //        BackgroundImage = new Uri("/Assets/Tiles/TileMediumAdd.png", UriKind.Relative),
        //    };

        //    Uri tileUri = new Uri(Constants.ADD_EXPENSE_TILE_SHORTCUT, UriKind.Relative);
        //    ShellTile.Create(tileUri, tileData);
        //}

        //returns true only if one or less group is selected
        private bool ValidGroupSelected()
        {
            if (this.expenseControl.groupList.SelectedItems.Count > 1)
                return false;
            else
                return true;
        }

        private async void GroupListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                var dialog = new MessageDialog("Sorry you can only select one group")
                {
                    Title = "Error"
                };
                await dialog.ShowAsync();

                //this.friendList.SelectedItems.Clear();
                this.expenseControl.groupList.SelectedItems.Clear();
            }
        }

        private async Task AddNewExpense()
        {
            ModifyDatabase modify = new ModifyDatabase(_addExpenseCompleted);
            await modify.AddExpense(this.expenseControl.expense);
        }

        private async void _addExpenseCompleted(bool success, HttpStatusCode errorCode)
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
                        MainPage.Current.Frame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        var dialog = new MessageDialog("Unable to add expense")
                        {
                            Title = "Error"
                        };
                        await dialog.ShowAsync();
                    }
                    commandBar.IsEnabled = true;
                });
            }
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