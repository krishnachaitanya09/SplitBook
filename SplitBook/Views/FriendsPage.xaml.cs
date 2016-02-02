using SplitBook.Add_Expense_Pages;
using SplitBook.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SplitBook.Views
{
    public sealed partial class FriendsPage : Page
    {
        public static FriendsPage Current;
        public FriendsPage()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) =>
            {
                Current = this;
            };
            BackButton.Visibility = Visibility.Collapsed;
            llsFriends.ItemsSource = MainPage.friendsList;
            balancePanel.DataContext = MainPage.netBalance;
            commandBar.DataContext = MainPage.buttonEnabler;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Frame.BackStack.Clear();
            if (MainPage.Current != null)
                MainPage.Current.NavMenuList.SelectedIndex = 0;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("FriendsPage");
            base.OnNavigatedTo(e);
        }

        private void ProfilePic_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }

        private void TotalBalance_Tapped(object sender, TappedRoutedEventArgs e)
        {
            llsFriends.ItemsSource = MainPage.balanceFriends;
            totalBalanceBox.BorderThickness = new Thickness(0, 0, 1, 0);
            youOweBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youAreOwedBox.BorderThickness = new Thickness(0, 0, 0, 2);
            filterText.Text = "Showing friends with balance";
            filterPanel.Visibility = Visibility.Visible;
        }

        private void YouOwed_Tapped(object sender, TappedRoutedEventArgs e)
        {
            llsFriends.ItemsSource = MainPage.youOweFriends;
            totalBalanceBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youOweBox.BorderThickness = new Thickness(0, 0, 1, 0);
            youAreOwedBox.BorderThickness = new Thickness(0, 0, 0, 2);
            filterText.Text = "Showing friends owe you";
            filterPanel.Visibility = Visibility.Visible;
        }

        private void YouAreOwed_Tapped(object sender, TappedRoutedEventArgs e)
        {
            llsFriends.ItemsSource = MainPage.owesYouFriends;
            totalBalanceBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youOweBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youAreOwedBox.BorderThickness = new Thickness(0, 0, 0, 0);
            filterText.Text = "Showing friends you owe";
            filterPanel.Visibility = Visibility.Visible;
        }

        private void FliterDone_Clicked(object sender, RoutedEventArgs e)
        {
            llsFriends.ItemsSource = MainPage.friendsList;
            totalBalanceBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youOweBox.BorderThickness = new Thickness(0, 0, 1, 2);
            youAreOwedBox.BorderThickness = new Thickness(0, 0, 0, 2);
            filterText.Text = "";
            filterPanel.Visibility = Visibility.Collapsed;
        }

        private void llsFriends_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;
            User selectedUser = llsFriends.SelectedItem as User;
            this.Frame.Navigate(typeof(UserDetails), selectedUser);
            llsFriends.SelectedItem = null;
            MainPage.Current.ResetNavMenu();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.FetchData();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExpenseSearch));
            MainPage.Current.ResetNavMenu();
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;
            this.Frame.Navigate(typeof(AddExpense));
            MainPage.Current.ResetNavMenu();
        }

        private void AddFriend_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateFriend));
            MainPage.Current.ResetNavMenu();
        }
    }
}
