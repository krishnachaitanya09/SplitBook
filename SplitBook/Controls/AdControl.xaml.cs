using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SplitBook.Controls
{
    public sealed partial class AdControl : UserControl
    {
        private Microsoft.Advertising.WinRT.UI.AdControl AdMediator;
        public AdControl()
        {
            AdMediator = new Microsoft.Advertising.WinRT.UI.AdControl();
            AdMediator.ErrorOccurred += AdMediator_ErrorOccurred;
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Advertisement.ShowAds)
            {
                AdMediator.Visibility = Visibility.Visible;
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    AdMediator.ApplicationId = "39ee0609-be6d-4158-b211-5b83d6ec32c3";
                    AdMediator.AdUnitId = "11664988";
                    AdMediator.Width = 480;
                    AdMediator.Height = 80;
                }
                else
                {

                    AdMediator.ApplicationId = "504c2e83-08d6-405f-a2a5-da731188fd85";
                    AdMediator.AdUnitId = "11664987";
                    AdMediator.Width = 728;
                    AdMediator.Height = 90;
                }
                AdMediator.IsAutoRefreshEnabled = true;
                adGrid.Children.Add(AdMediator);
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void AdMediator_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            AdMediator.Visibility = Visibility.Collapsed;
            AdMediator.IsAutoRefreshEnabled = false;
        }
    }
}
