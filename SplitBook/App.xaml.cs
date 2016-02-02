using SplitBook.Model;
using SplitBook.Utilities;
using SplitBook.Views;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SplitBook
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static string accessToken, accessTokenSecret;
        public static User currentUser;
        public Expense ADD_EXPENSE { get; set; }
        public Expense SELECTED_EXPENSE { get; set; }
        public Group SELECTED_GROUP { get; set; }
        public User PAYMENT_USER { get; set; }
        public int PAYMENT_TYPE { get; set; }
        public int PAYMENT_GROUP { get; set; }
        public User NEW_USER { get; set; }
        public Group NEW_GROUP { get; set; }        

        public Frame rootFrame { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;
            this.UnhandledException += App_UnhandledException; 
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendException(e.Message, true);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {

                    titleBar.ButtonBackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color;
                    titleBar.ButtonHoverBackgroundColor = (Application.Current.Resources["splitwiseGreenHover"] as SolidColorBrush).Color;
                    titleBar.ButtonPressedBackgroundColor = (Application.Current.Resources["splitwiseGreenPressed"] as SolidColorBrush).Color;
                    titleBar.ButtonForegroundColor = Color.FromArgb(255, 255, 255, 254);
                    titleBar.BackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color; ;
                    titleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 254);
                }
            }

            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = (Application.Current.Resources["splitwiseGreen"] as SolidColorBrush).Color;
                    statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 254);
                }
            }

            Advertisement.UpdateInAppPurchases();

            //#if DEBUG
            //            if (System.Diagnostics.Debugger.IsAttached)
            //            {
            //                this.DebugSettings.EnableFrameRateCounter = true;
            //            }
            //#endif
            Helpers dbHelper = new Helpers();
            dbHelper.CreateDatabase();

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                App.accessToken = Helpers.AccessToken;
                App.accessTokenSecret = Helpers.AccessTokenSecret;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_TAG] != null &&
                    ApplicationData.Current.LocalSettings.Values[Constants.ACCESS_TOKEN_SECRET_TAG] != null)
                {
                    rootFrame.Navigate(typeof(MainPage), false);
                }
                else
                {
                    rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                }

            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnResuming(object sender, object e)
        {

            if (String.IsNullOrEmpty(App.accessToken))
            {
                Debug.WriteLine("App to foreground. App token is null");
                App.accessToken = Helpers.AccessToken;
                App.accessTokenSecret = Helpers.AccessTokenSecret;
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
