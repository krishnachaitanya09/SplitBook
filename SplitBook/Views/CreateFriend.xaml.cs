using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
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
    public sealed partial class CreateFriend : Page
    {
        string email, firstName, lastName;

        public CreateFriend()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            MainPage.Current.ResetNavMenu();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("CreateFriendPage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsActive = true;
            this.Focus(FocusState.Programmatic);
            email = tbEmail.Text;
            firstName = tbFirstName.Text;
            lastName = tbLastName.Text;
            await CreateFriendAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void EnableOkButton()
        {
            if (Helpers.IsValidEmail(tbEmail.Text) && !String.IsNullOrEmpty(tbFirstName.Text))
                okay.IsEnabled = true;
            else
                okay.IsEnabled = false;
        }

        private async Task CreateFriendAsync()
        {
            ModifyDatabase modify = new ModifyDatabase(_addFriendCompleted);
            await modify.CreateFriend(email, firstName, lastName);
        }

        private void TbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOkButton();
        }

        private async void BtnImport_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var contactPicker = new ContactPicker();
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email);
            Contact contact = await contactPicker.PickContactAsync();
            if (contact != null)
            {
                tbEmail.Text = contact.Emails[0].Address;
                tbFirstName.Text = contact.DisplayName;
            }
        }

        private async void _addFriendCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {

                    User friend = (Application.Current as App).NEW_USER as User;
                    //App.friendsList.Add(friend);
                    (Application.Current as App).NEW_USER = null;
                    busyIndicator.IsActive = false;
                    if (this.Frame.CanGoBack)
                        this.Frame.GoBack();
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("User has successfully been added as friend.", "Success");
                        await messageDialog.ShowAsync();
                    }
                    await MainPage.Current.FetchData();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;
                    MessageDialog messageDialog = new MessageDialog("Unable to add user", "Error");
                    await messageDialog.ShowAsync();
                });
            }
        }

        private void TbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOkButton();
        }
    }
}
