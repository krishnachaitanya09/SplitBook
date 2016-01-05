using SplitWisely.Controller;
using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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


namespace SplitWisely.Views
{
    public sealed partial class CreateFriend : Page
    {
        BackgroundWorker createFriendBackgroundWorker;
        string email, firstName, lastName;

        public CreateFriend()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            MainPage.Current.ResetNavMenu();
            createFriendBackgroundWorker = new BackgroundWorker();
            createFriendBackgroundWorker.WorkerSupportsCancellation = true;
            createFriendBackgroundWorker.DoWork += new DoWorkEventHandler(createFriendBackgroundWorker_DoWork);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!createFriendBackgroundWorker.IsBusy)
            {
                busyIndicator.IsActive = true;
                this.Focus(FocusState.Programmatic);
                email = tbEmail.Text;
                firstName = tbFirstName.Text;
                lastName = tbLastName.Text;
                createFriendBackgroundWorker.RunWorkerAsync();
            }
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

        private void createFriendBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_addFriendCompleted);
            modify.createFriend(email, firstName, lastName);
        }

        private void tbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOkButton();
        }

        private async void btnImport_Tapped(object sender, TappedRoutedEventArgs e)
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

        private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOkButton();
        }
    }
}
