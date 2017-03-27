using SplitBook.Add_Expense_Pages;
using SplitBook.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;


namespace SplitBook.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpensePage : Page
    {
        public ExpensePage()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;
            llsExpenses.ItemsSource = MainPage.expensesList;            
            commandBar.DataContext = MainPage.buttonEnabler;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainPage.Current.NavMenuList.SelectedIndex = 2;
            BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("ExpensePage");
        }

        private void LlsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));

            llsExpenses.SelectedItem = null;
            MainPage.Current.ResetNavMenu();
        }

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListViewBase listview)
            {
                // Attach to the view changed event
                var _scrollViewer = listview.GetFirstDescendantOfType<ScrollViewer>();
                if (_scrollViewer != null)
                {
                    _scrollViewer.ViewChanged += OnViewChanged;
                }
            }
        }

        private async void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var _scrollViewer = sender as ScrollViewer;
            // If scrollviewer is scrolled down at least 90%
            if (_scrollViewer.VerticalOffset > Math.Max(_scrollViewer.ScrollableHeight * 0.6, _scrollViewer.ScrollableHeight - 200))
            {
                if (MainPage.morePages)
                {
                    MainPage.pageNo++;
                    await MainPage.Current.LoadExpenses();
                }
            }
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ADD_EXPENSE = null;
            this.Frame.Navigate(typeof(AddExpense));
            MainPage.Current.ResetNavMenu();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await MainPage.Current.FetchData();
            MainPage.Current.ResetNavMenu();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExpenseSearch));
            MainPage.Current.ResetNavMenu();
        }
    }
}
