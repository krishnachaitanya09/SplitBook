using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace SplitBook.Views
{
    public sealed partial class DebtSimplification : Page
    {
        CheckBox doNotShowCheckBox;
        public DebtSimplification()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            Uri url = new Uri("https://secure.splitwise.com/users/simplify_debts");
            browser.Navigate(url);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            busyIndicator.IsActive = true;
        }

        private async void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            busyIndicator.IsActive = false;
            if (args.Uri.ToString().Contains("edit"))
            {
                Debug.WriteLine("Simplification done. Show prompt to exit");

                MessageDialog messageDialog = new MessageDialog("Debt simplification was successful", "Success");
                await messageDialog.ShowAsync();

                if (this.Frame.CanGoBack)
                    this.Frame.GoBack();
            }
        }

        private void browser_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            busyIndicator.IsActive = false;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helpers.doNotShowDebtSimplificationBox())
                return;

            string hey = "Hey there!";
            string message = "After you are done with debt simplification, refresh the app to reflect the changes.";
            //MessageBox.Show(message, hey, MessageBoxButton.OK);
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap
            });

            doNotShowCheckBox = new CheckBox()
            {
                Content = "Do not show again",
                IsChecked = false
            };

            panel.Children.Add(doNotShowCheckBox);

            ContentDialog dialog = new ContentDialog()
            {
                Title = hey,
                Content = panel,

            };
            dialog.PrimaryButtonText = "OK";
            dialog.IsPrimaryButtonEnabled = false;

            await dialog.ShowAsync();
            if (doNotShowCheckBox.IsChecked.Value)
            {
                Helpers.setDonNotShowDebtSimplifationBox();
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
    }
}
