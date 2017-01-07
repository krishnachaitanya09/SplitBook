using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace SplitBook.Views
{
    public sealed partial class MePage : Page
    {
        public MePage()
        {
            this.InitializeComponent();
            BackButton.Tapped += BackButton_Tapped;
            if (!Advertisement.ShowAds)
            {
                removeAds.Visibility = Visibility.Collapsed;
            }
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
            GoogleAnalytics.EasyTracker.GetTracker().SendView("MePage");
        }

        private void Account_Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AccountSettings));
        }

        private void Simplify_Debt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DebtSimplification));
        }

        private async void Rate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uriRate = new Uri(@"ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(uriRate);
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "Rate_Click", "Rate", 0);
        }

        private void RemoveAds_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));            
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "RemoveAds_Click", "RemoveAds", 0);
        }

        private void ProfilePic_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as BitmapImage;
            profilePic.UriSource = new Uri("ms-appx:///Assets/Images/profilePhoto.png");
        }

        private void Logout_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Helpers.logout();
            (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
        }

        private async void Contactus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResourceLoader loader = new ResourceLoader();
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();

            PackageVersion PackageVersion = Package.Current.Id.Version;

            EmailMessage emailComposeTask = new EmailMessage();
            emailComposeTask.Subject = "SplitBook Feedback";
            emailComposeTask.Body = String.Format(loader.GetString("FeedbackBody"), deviceInfo.SystemProductName, deviceInfo.FriendlyName,
                deviceInfo.OperatingSystem, deviceInfo.SystemManufacturer, deviceInfo.SystemFirmwareVersion, deviceInfo.SystemHardwareVersion,
                 string.Format("{0}.{1}.{2}.{3}", PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build, PackageVersion.Revision));
            emailComposeTask.To.Add(new EmailRecipient("contact@techcryptic.com"));
            await EmailManager.ShowComposeNewEmailAsync(emailComposeTask);
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "Contact_Click", "Contact", 0);
        }
    }
}
