using SplitBook.Utilities;
using SplitBook.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI;
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
                Visibility = Visibility.Visible;
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
                Button removeButton = new Button()
                {
                    Content = new FontIcon()
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8BB",
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 12
                    },
                    Padding = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0))
                };
                removeButton.Click += RemoveButton_Click;

                adGrid.Children.Add(removeButton);
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.frame.Navigate(typeof(AboutPage));
        }

        private void AdMediator_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            AdMediator.IsAutoRefreshEnabled = false;
        }
    }
}
