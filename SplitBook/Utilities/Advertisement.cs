using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace SplitBook.Utilities
{
    static class Advertisement
    {
        public static bool ShowAds { get; set; }
        public static bool NoAdsIsActive { get; set; }
        public static bool Donate5IsActive { get; set; }
        public static bool Donate10IsActive { get; set; }

        public static void UpdateInAppPurchases()
        {
            ShowAds = true;
            try
            {
#if DEBUG
                NoAdsIsActive = CurrentAppSimulator.LicenseInformation.ProductLicenses["NoAds"].IsActive;
                Donate5IsActive = CurrentAppSimulator.LicenseInformation.ProductLicenses["Donate5"].IsActive;
                Donate10IsActive = CurrentAppSimulator.LicenseInformation.ProductLicenses["Donate10"].IsActive;
#else
                NoAdsIsActive = CurrentApp.LicenseInformation.ProductLicenses["NoAds"].IsActive;
                Donate5IsActive = CurrentApp.LicenseInformation.ProductLicenses["Donate5"].IsActive;
                Donate10IsActive = CurrentApp.LicenseInformation.ProductLicenses["Donate10"].IsActive;
#endif

                if (NoAdsIsActive || Donate5IsActive || Donate10IsActive)
                {
                    ShowAds = false;
                }
            }
            catch (Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
            }

        }

        public static async void PurchaseRemoveAds()
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["NoAds"].IsActive)
            {
                try
                {
                    PurchaseResults purchaseResults = await CurrentApp.RequestProductPurchaseAsync("NoAds");
                    if (purchaseResults.Status == ProductPurchaseStatus.Succeeded)
                        UpdateInAppPurchases();
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception ex)
                {
                    GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
        }

        public static async void PurchaseDonate5()
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["Donate5"].IsActive)
            {
                try
                {
                    PurchaseResults purchaseResults = await CurrentApp.RequestProductPurchaseAsync("Donate5");
                    if (purchaseResults.Status == ProductPurchaseStatus.Succeeded)
                        UpdateInAppPurchases();
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception ex)
                {
                    GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
        }

        public static async void PurchaseDonate10()
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["Donate10"].IsActive)
            {
                try
                {
                    PurchaseResults purchaseResults = await CurrentApp.RequestProductPurchaseAsync("Donate10");
                    if (purchaseResults.Status == ProductPurchaseStatus.Succeeded)
                        UpdateInAppPurchases();
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception ex)
                {
                    GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message + ":" + ex.StackTrace, false);
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
        }

        public static async Task<ListingInformation> GetAddons()
        {
#if DEBUG
            return await CurrentAppSimulator.LoadListingInformationAsync();
#else
            return await CurrentApp.LoadListingInformationAsync();
#endif
        }
    }
}

