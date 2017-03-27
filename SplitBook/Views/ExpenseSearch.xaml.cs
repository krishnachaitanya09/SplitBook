using SplitBook.Controller;
using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace SplitBook.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpenseSearch : Page
    {
        ObservableCollection<Expense> expenseSearchResult = new ObservableCollection<Expense>();

        public ExpenseSearch()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;        
            llsExpenses.ItemsSource = expenseSearchResult;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("ExpenseSearchPage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void LlsExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));

            llsExpenses.SelectedItem = null;
        }

        private async void Query_Submitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            string searchText = sender.QueryText;
            if (!String.IsNullOrEmpty(searchText))
                await Search(searchText);
        }

        private  async Task Search(string text)
        {
            this.Focus(FocusState.Programmatic);
            busyIndicator.IsActive = true;
            expenseSearchResult.Clear();
            await SearchExpenseAsync(text);
        }

        private async Task SearchExpenseAsync(string text)
        {
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.SearchForExpense(text);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (allExpenses != null)
                {
                    foreach (var expense in allExpenses)
                    {
                        expenseSearchResult.Add(expense);
                    }
                }
                busyIndicator.IsActive = false;
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus(FocusState.Programmatic);
        }
    }
}
