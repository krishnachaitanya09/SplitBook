using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitWisely.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color; ;
                    titleBar.ForegroundColor = Colors.White;
                }
            }

            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color; ;
                    statusBar.ForegroundColor = Colors.White;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavMenuList.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            // After a successful navigation set keyboard focus to the loaded page
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }
        }

        private void NavMenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RootSplitView.IsPaneOpen)
                RootSplitView.IsPaneOpen = false;

            switch (NavMenuList.SelectedIndex)
            {
                case 0:
                    this.frame.Navigate(typeof(FriendsPage), false);
                    pageTitle.Text = "Friends";
                    break;
                case 1:
                    this.frame.Navigate(typeof(GroupsPage));
                    pageTitle.Text = "Groups";
                    break;
                case 2:
                    this.frame.Navigate(typeof(ActivityPage));
                    pageTitle.Text = "Activity";
                    break;
            }
        }

        private void TogglePaneButton_Clicked(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
        }
    }
}
