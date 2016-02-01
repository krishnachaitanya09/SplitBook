using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace SplitBook.Views
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public static ObservableCollection<User> balanceFriends = new ObservableCollection<User>();
        public static ObservableCollection<User> youOweFriends = new ObservableCollection<User>();
        public static ObservableCollection<User> owesYouFriends = new ObservableCollection<User>();
        public static ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        public static ObservableCollection<Group> groupsList = new ObservableCollection<Group>();
        public static ObservableCollection<User> friendsList = new ObservableCollection<User>();
        private object o = new object();

        private double postiveBalance = 0, negativeBalance = 0, totalBalance = 0;
        public static int pageNo = 0;
        public static bool morePages = true, hasDataLoaded = false;
        public static NetBalances netBalance = new NetBalances();
        public static ButtonEnabler buttonEnabler = new ButtonEnabler();
        public static BackgroundWorker expenseLoadingBackgroundWorker = new BackgroundWorker();
        public static BackgroundWorker groupLoadingBackgroundWorker = new BackgroundWorker();
        public static BackgroundWorker syncDatabaseBackgroundWorker = new BackgroundWorker();
        SyncDatabase databaseSync;

        public Frame AppFrame { get { return this.frame; } }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) =>
            {
                Current = this;
                this.TogglePaneButton.Focus(FocusState.Programmatic);
            };

            this.RootSplitView.RegisterPropertyChangedCallback(
               SplitView.DisplayModeProperty,
               (s, a) =>
               {
                   // Ensure that we update the reported size of the TogglePaneButton when the SplitView's
                   // DisplayMode changes.
                   this.CheckTogglePaneButtonSizeChanged();
               });

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            expenseLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            expenseLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(expenseLoadingBackgroundWorker_DoWork);
            if (expenseLoadingBackgroundWorker.IsBusy != true)
            {
                expenseLoadingBackgroundWorker.RunWorkerAsync();
            }

            groupLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            groupLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(groupLoadingBackgroundWorker_DoWork);
            if (groupLoadingBackgroundWorker.IsBusy != true)
            {
                groupLoadingBackgroundWorker.RunWorkerAsync();
            }

            populateData();

            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            me.DataContext = App.currentUser;
            NavMenuList.SelectedIndex = 0;
            this.frame.Navigate(typeof(FriendsPage));
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            else
            {
                bool firstUse = (bool)e.Parameter;
                databaseSync = new SyncDatabase(SyncCompleted);
                //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
                //if the page has been accessed from the back stack
                if (firstUse)
                {
                    databaseSync.isFirstSync(true);

                    //disable the add expense and searchtill first sycn is complete
                    buttonEnabler.AddButtonEnabled = false;
                    buttonEnabler.SearchButtonEnabled = false;
                }

                if (syncDatabaseBackgroundWorker.IsBusy != true)
                {
                    busyIndicator.Visibility = Visibility.Visible;
                    syncDatabaseBackgroundWorker.RunWorkerAsync();
                    buttonEnabler.RefreshButtonEnabled = false;
                }
            }
        }

        private void expenseLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
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
            netBalance.setBalances(App.currentUser.default_currency, totalBalance, postiveBalance, negativeBalance);
        }

        private void populateData()
        {
            loadFriends();
            if (expenseLoadingBackgroundWorker.IsBusy != true)
            {
                expenseLoadingBackgroundWorker.RunWorkerAsync();
            }

            if (groupLoadingBackgroundWorker.IsBusy != true)
            {
                groupLoadingBackgroundWorker.RunWorkerAsync();
            }
        }

        private async void loadExpenses()
        {
            List<Expense> allExpenses;
            lock (o)
            {
                QueryDatabase obj = new QueryDatabase();
                allExpenses = obj.getAllExpenses(pageNo);
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (pageNo == 0)
                    expensesList.Clear();

                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;
                else
                    morePages = true;

                if (allExpenses != null)
                {
                    foreach (var expense in allExpenses)
                    {
                        expensesList.Add(expense);
                    }
                }
            });
        }

        private async void loadGroups()
        {
            QueryDatabase obj = new QueryDatabase();
            List<Group> allGroups = obj.getAllGroups();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                groupsList.Clear();
                if (allGroups != null)
                {
                    foreach (var group in allGroups)
                    {
                        groupsList.Add(group);
                    }
                }
            });
        }

        public void FetchData()
        {
            if (databaseSync == null)
                databaseSync = new SyncDatabase(SyncCompleted);

            busyIndicator.Visibility = Visibility.Visible;
            buttonEnabler.RefreshButtonEnabled = false;

            if (!syncDatabaseBackgroundWorker.IsBusy)
                syncDatabaseBackgroundWorker.RunWorkerAsync();
        }


        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && this.AppFrame.CanGoBack)
            {
                e.Handled = true;
                this.AppFrame.GoBack();
            }
        }


        private void syncDatabaseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            databaseSync.performSync();
        }

        private void groupLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadGroups();
        }

        private async void SyncCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    busyIndicator.Visibility = Visibility.Collapsed;
                    pageNo = 0;
                    populateData();
                    hasDataLoaded = true;
                    me.DataContext = App.currentUser; 

                    buttonEnabler.AddButtonEnabled = true;
                    buttonEnabler.SearchButtonEnabled = true;
                    buttonEnabler.RefreshButtonEnabled = true;
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    buttonEnabler.RefreshButtonEnabled = true;

                    //    // don't need to handle the below two as there two are only disabled on first launch. If first launch sync fails, then these two buttons cannot be activated.
                    buttonEnabler.AddButtonEnabled = true;
                    buttonEnabler.SearchButtonEnabled = true;

                    busyIndicator.Visibility = Visibility.Collapsed;
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


        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            // After a successful navigation set keyboard focus to the loaded page
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
            this.CheckTogglePaneButtonSizeChanged();           
        }

        private void NavMenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RootSplitView.IsPaneOpen)
                RootSplitView.IsPaneOpen = false;

            switch (NavMenuList.SelectedIndex)
            {
                case 0:
                    
                    SecondaryNavMenuList.SelectedIndex = -1;
                    break;
                case 1:
                    
                    SecondaryNavMenuList.SelectedIndex = -1;
                    break;
                case 2:
                    
                    SecondaryNavMenuList.SelectedIndex = -1;
                    break;
                default:
                    break;
            }
        }

        private void TogglePaneButton_Clicked(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
        }

        public void ResetNavMenu()
        {
            NavMenuList.SelectedIndex = -1;
            SecondaryNavMenuList.SelectedIndex = -1;
        }

        public Rect TogglePaneButtonRect
        {
            get;
            private set;
        }

        private void ProfilePic_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }

        private void SecondaryNavMenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RootSplitView.IsPaneOpen)
                RootSplitView.IsPaneOpen = false;

            switch (SecondaryNavMenuList.SelectedIndex)
            {
                case 0:
                   
                    NavMenuList.SelectedIndex = -1;
                    break;
                case 1:                   
                    NavMenuList.SelectedIndex = -1;
                    break;
                default:
                    break;
            }
        }

        private void FriendsMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.frame.Navigate(typeof(FriendsPage), false);
        }

        private void GroupsMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.frame.Navigate(typeof(GroupsPage));
        }

        private void ExpensesMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.frame.Navigate(typeof(ExpensePage));
        }

        private void MeMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.frame.Navigate(typeof(MePage));
        }

        private void LogOut_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Helpers.logout();
            (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        /// An event to notify listeners when the hamburger button may occlude other content in the app.
        /// The custom "PageHeader" user control is using this.
        /// </summary>
        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            if (handler != null)
            {
                // handler(this, this.TogglePaneButtonRect);
                handler.DynamicInvoke(this, this.TogglePaneButtonRect);
            }
        }
    }
}
