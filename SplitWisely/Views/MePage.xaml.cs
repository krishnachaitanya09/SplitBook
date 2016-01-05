using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace SplitWisely.Views
{
    public sealed partial class MePage : Page
    {
        public MePage()
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
            MainPage.Current.SecondaryNavMenuList.SelectedIndex = 0;
            Me.DataContext = App.currentUser;
        }

        private void Account_Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Simplify_Debt_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void Rate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uriRate = new Uri(@"ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(uriRate);
        }

        private void RemoveAds_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void ProfilePic_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
