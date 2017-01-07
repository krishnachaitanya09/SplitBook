using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class AboutPage : Page
    {
        private ListingInformation listingInfo;
        private string noAdsPrice;
        private string donate5Price;
        private string donate10Price;

        public AboutPage()
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainPage.Current.SecondaryNavMenuList.SelectedIndex = 1;
            await LoadListingInformation();
            LoadPurchaseOptions();
            if (!Advertisement.ShowAds)
            {
                //removeAds.Visibility = Visibility.Collapsed;
            }
            PackageVersion PackageVersion = Package.Current.Id.Version;
            version.Text = string.Format("{0}.{1}.{2}.{3}", PackageVersion.Major, PackageVersion.Minor, PackageVersion.Build, PackageVersion.Revision);
            GoogleAnalytics.EasyTracker.GetTracker().SendView("AboutPage");
        }

        private async Task LoadListingInformation()
        {
            listingInfo = await Advertisement.GetAddons();
            noAdsPrice = listingInfo.ProductListings["NoAds"].FormattedPrice;
            donate5Price = listingInfo.ProductListings["Donate5"].FormattedPrice;
            donate10Price = listingInfo.ProductListings["Donate10"].FormattedPrice;
        }

        private void LoadPurchaseOptions()
        {
            removeAdsText.Text = $"App development takes a lot of time and effort. To support the development of this app, I monetize with ads. You can remove the ads by purchasing premium features for {noAdsPrice}.";
            donateText.Text = $"Another way to get premium features is by donating {donate5Price} or {donate10Price}. This way you'll also support SplitBook's further development.";
            removeAdsButton.Content = $"Remove ads for {noAdsPrice}";
            donate5Button.Content = $"Donate {donate5Price}";
            donate10Button.Content = $"Donate {donate10Price}";
            if (Advertisement.NoAdsIsActive || Advertisement.Donate5IsActive || Advertisement.Donate10IsActive)
            {
                removeAdsText.Text = "Thanks for purchasing SplitBook. All the premium features are now available to you.";
                removeAdsButton.Visibility = Visibility.Collapsed;
                if (Advertisement.Donate5IsActive)
                {
                    donate5Button.Visibility = Visibility.Collapsed;
                }
                if (Advertisement.Donate10IsActive)
                {
                    donate10Button.Visibility = Visibility.Collapsed;
                }
                if (Advertisement.Donate5IsActive && Advertisement.Donate10IsActive)
                {
                    donateText.Text = "Thanks a lot for donating us. We really appreciate your support for SplitBook's further developement. If you want to donate any more money, please contact us.";
                }
            }

        }

        private async void Rate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uriRate = new Uri(@"ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(uriRate);
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "Rate_Click", "Rate", 0);
        }

        private void Contactus_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            ContactUs();
        }

        private void Contactus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContactUs();
        }

        private async void ContactUs()
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

        private void RemoveAdsButton_Click(object sender, RoutedEventArgs e)
        {
            Advertisement.PurchaseRemoveAds();
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "RemoveAds_Click", "RemoveAds", 0);
        }

        private void Donate5Button_Click(object sender, RoutedEventArgs e)
        {
            Advertisement.PurchaseDonate5();
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "Donate5_Click", "Donate5", 0);
        }

        private void Donate10Button_Click(object sender, RoutedEventArgs e)
        {
            Advertisement.PurchaseDonate10();
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("UI", "Donate10_Click", "Donate10", 0);
        }
    }
}
