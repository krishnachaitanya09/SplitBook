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
            try {
                var IsActive = CurrentApp.LicenseInformation.ProductLicenses["NoAds"].IsActive;
                if (IsActive)
                {
                    ShowAds = false;
                }
            }
           catch(Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
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
                    GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
        }
    }
}

