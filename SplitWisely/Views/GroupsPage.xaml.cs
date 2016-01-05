using SplitWisely.Add_Expense_Pages;
using SplitWisely.Controller;
using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class GroupsPage : Page
    {
        public GroupsPage()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            llsGroups.ItemsSource = MainPage.groupsList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void llsGroups_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsGroups.SelectedItem == null)
                return;
            Group selectedGroup = llsGroups.SelectedItem as Group;

            (Application.Current as App).SELECTED_GROUP = selectedGroup;
            this.Frame.Navigate(typeof(GroupDetailsPage));

            llsGroups.SelectedItem = null;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;
            this.Frame.Navigate(typeof(AddExpense));
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
