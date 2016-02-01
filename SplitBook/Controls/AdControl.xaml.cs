using SplitBook.Utilities;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SplitBook.Controls
{
    public sealed partial class AdControl : UserControl
    {
        public AdControl()
        {
            this.InitializeComponent();
        }

        private void AdMediator_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            AdMediator.Visibility = Visibility.Collapsed;
        }

        private void AdMediator_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        {
            AdMediator.Visibility = Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Advertisement.ShowAds)
            {
                AdMediator.Visibility = Visibility.Visible;                
            }
            else
            {
                Visibility = Visibility.Collapsed;                
            }            
        }
    }
}
