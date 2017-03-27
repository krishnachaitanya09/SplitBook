using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class AccountSettings : Page
    {
        private ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        User currentUser;
        bool currencyModified = false;

        public AccountSettings()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;

            currencyListPicker.AddHandler(TappedEvent, new TappedEventHandler(CurrencyListPicker_Tapped), true);

            currentUser = new User()
            {
                id = App.currentUser.id,
                first_name = App.currentUser.first_name,
                last_name = App.currentUser.last_name,
                email = App.currentUser.email,
                default_currency = App.currentUser.default_currency
            };
            this.DataContext = currentUser;

            this.currencyList.ItemsSource = currenciesList;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await GetSupportedCurrencies();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("AccountSettingsPage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private bool CanProceed()
        {
            return !(String.IsNullOrEmpty(tbEmail.Text) || String.IsNullOrEmpty(tbFirstName.Text));
        }

        private async Task EditUser()
        {
            Request.UpdateUserRequest request = new Request.UpdateUserRequest(currentUser);
            await request.UpdateUser(EditUserCompleted);
        }

        private async void EditUserCompleted(User updatedUserDetails, HttpStatusCode statusCode)
        {
            if (updatedUserDetails != null && statusCode == HttpStatusCode.OK)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    App.currentUser = updatedUserDetails;
                    busyIndicator.IsActive = false;
                    ShowPromptAndGoBack();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;

                    if (statusCode == HttpStatusCode.Unauthorized)
                    {
                        (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Unable to edit user details.", "Error");
                        await messageDialog.ShowAsync();
                    }
                });
            }
        }

        private async void ShowPromptAndGoBack()
        {
            if (currencyModified)
            {
                MessageDialog messageDialog = new MessageDialog("In order to set exisiting expenses to new default currency, please go to Account Settings on Splitwise.com", "Success");
                await messageDialog.ShowAsync();
            }
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async Task GetSupportedCurrencies()
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.GetSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency)
                    defaultCurrency = item;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    currenciesList.Add(item);
                });
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.currencyList.SelectedItem = defaultCurrency;
            });
        }

        private void CurrencyListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currencyListPicker.Text = CurrencyListSummary();
        }

        private string CurrencyListSummary()
        {
            string summary = String.Empty;
            Currency currency = (Currency)currencyList.SelectedItem;
            summary = String.Concat(summary, currency.currency_code);
            return summary;
        }

        private void CurrencyListPicker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.currencyListPopup.IsOpen = true;
        }

        private async void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanProceed())
            {
                busyIndicator.IsActive = true;
                currentUser.first_name = tbFirstName.Text;
                currentUser.last_name = tbLastName.Text;
                currentUser.email = tbEmail.Text;
                Currency selectedCurrency = (currencyList.SelectedItem as Currency);

                if (selectedCurrency != null)
                    currentUser.default_currency = selectedCurrency.currency_code;

                if (App.currentUser != null && !currentUser.default_currency.Equals(App.currentUser.default_currency))
                    currencyModified = true;
                await Task.Run(async () =>
                {
                    await EditUser();
                });                
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Email and First Name cannot be empty", "Error");
                await messageDialog.ShowAsync();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
