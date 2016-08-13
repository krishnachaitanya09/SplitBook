using SplitBook.Controller;
using SplitBook.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace SplitBook.Views
{
    public sealed partial class CreateGroup : Page
    {
        BackgroundWorker createGroupBackgroundWorker;
        BackgroundWorker friendLoadingBackgroundWorker;

        Group groupToAdd;

        ObservableCollection<User> groupMembers = new ObservableCollection<User>();
        ObservableCollection<User> friendsList = new ObservableCollection<User>();

        public CreateGroup()
        {
            this.InitializeComponent();

            groupToAdd = new Group();

            friendListPicker.AddHandler(TappedEvent, new TappedEventHandler(friendListPicker_Tapped), true);
            BackButton.Click += BackButton_Click;
            createGroupBackgroundWorker = new BackgroundWorker();
            createGroupBackgroundWorker.WorkerSupportsCancellation = true;
            createGroupBackgroundWorker.DoWork += new DoWorkEventHandler(createGroupBackgroundWorker_DoWork);

            friendLoadingBackgroundWorker = new BackgroundWorker();
            friendLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            friendLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(friendLoadingBackgroundWorker_DoWork);
            friendLoadingBackgroundWorker.RunWorkerAsync();

            this.friendList.ItemsSource = friendsList;

            groupMembers.Add(App.currentUser);
            llsFriends.ItemsSource = groupMembers;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("CreateGroupPage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void friendLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueryDatabase obj = new QueryDatabase();
            List<User> allFriends = obj.getAllFriends();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (allFriends != null)
                {
                    foreach (var group in allFriends)
                    {
                        friendsList.Add(group);
                    }
                }
            });
        }

        private string FriendSummary()
        {
            string summary = String.Empty;
            /*for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                User friend = (User)list[i];
                summary = String.Concat(summary, friend.first_name);
                summary += isLast ? string.Empty : ", ";
            }*/

            if (friendList.SelectedItems.Count != 0)
                summary = friendList.SelectedItems.Count + " users selected";
            if (summary == String.Empty)
            {
                summary = "no friends selected";
            }
            return summary;
        }

        private void createGroupBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(CreateGroupCompleted);
            modify.createGroup(groupToAdd);
        }

        private async void CreateGroupCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Group group = (Application.Current as App).NEW_GROUP as Group;
                    //App.groupsList.Add(group);
                    (Application.Current as App).NEW_GROUP = null;
                    busyIndicator.IsActive = false;
                    if (this.Frame.CanGoBack)
                        this.Frame.GoBack();
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Group has been successfully created.", "Success");
                        await messageDialog.ShowAsync();
                    }
                    MainPage.Current.FetchData();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;
                    MessageDialog messageDialog = new MessageDialog("Unable to create group", "Error");
                    await messageDialog.ShowAsync();
                });
            }
        }

        private void EnableOkButton()
        {
            if (!String.IsNullOrEmpty(tbName.Text))
            okay.IsEnabled = true;
            else
            okay.IsEnabled = false;
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            groupToAdd.name = tbName.Text;
            EnableOkButton();
        }

        private void friendListPicker_Loaded(object sender, RoutedEventArgs e)
        {
            friendListPicker.Text = FriendSummary();
        }

        private void friendListPicker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            friendListPopup.IsOpen = true;
        }

        private void FriendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.friendListPicker.Text = this.FriendSummary();
            groupMembers.Clear();
            groupMembers.Add(App.currentUser);

            if (this.friendList.SelectedItems == null)
                return;
            foreach (var item in this.friendList.SelectedItems)
            {
                User user = item as User;
                groupMembers.Add(user);
            }
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!createGroupBackgroundWorker.IsBusy)
            {
                busyIndicator.IsActive = true;
                this.Focus(FocusState.Programmatic);
                groupToAdd.members = groupMembers.ToList();
                createGroupBackgroundWorker.RunWorkerAsync();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
