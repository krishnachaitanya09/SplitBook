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
    }
}

