using SplitWisely.Controller;
using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ExpenseSearch : Page
    {
        ObservableCollection<Expense> expenseSearchResult = new ObservableCollection<Expense>();
        BackgroundWorker searchExpenseBackgroundWorker;

        public ExpenseSearch()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;

            searchExpenseBackgroundWorker = new BackgroundWorker();
            searchExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            searchExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(searchExpenseBackgroundWorker_DoWork);

            llsExpenses.ItemsSource = expenseSearchResult;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void llsExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));

            llsExpenses.SelectedItem = null;
        }

        private void Query_Submitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            string searchText = sender.QueryText;
            if (!String.IsNullOrEmpty(searchText))
                search(searchText);
        }

        private void search(string text)
        {
            this.Focus(FocusState.Programmatic);
            busyIndicator.IsActive = true;
            expenseSearchResult.Clear();
            if (!searchExpenseBackgroundWorker.IsBusy)
                searchExpenseBackgroundWorker.RunWorkerAsync(text);
        }

        private async void searchExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.searchForExpense(e.Argument.ToString());
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
    }
}
