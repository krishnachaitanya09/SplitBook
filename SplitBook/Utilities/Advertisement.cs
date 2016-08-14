using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace SplitBook.Utilities
{
    static class Advertisement
    {
        public static bool ShowAds { get; set; }
        public static void UpdateInAppPurchases()
        {
            ShowAds = true;
            var IsActive = CurrentAppSimulator.LicenseInformation.ProductLicenses["NoAds"].IsActive;
            if (IsActive)
            {
                ShowAds = false;
            }
        }

        public static async void PurchaseRemoveAds()
        {
            if (!CurrentAppSimulator.LicenseInformation.ProductLicenses["NoAds"].IsActive)
            {
                try
                {
                    PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("NoAds");
                    if (purchaseResults.Status == ProductPurchaseStatus.Succeeded)
                        UpdateInAppPurchases();
                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception e)
                {
                    GoogleAnalytics.EasyTracker.GetTracker().SendException(e.Message, false);
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
        }
    }
}

