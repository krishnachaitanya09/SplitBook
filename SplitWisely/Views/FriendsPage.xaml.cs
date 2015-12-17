using SplitWisely.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;


namespace SplitWisely.Views
{
    public sealed partial class FriendsPage : Page
    {
        public FriendsPage()
        {
            this.InitializeComponent();

            llsFriends.ItemsSource = MainPage.friendsList;
            balancePanel.DataContext = MainPage.netBalanceObj;
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
        }
    }
}
