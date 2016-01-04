using SplitWisely.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;


namespace SplitWisely.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpensePage : Page
    {
        public ExpensePage()
        {
            this.InitializeComponent();
            this.PageHeader.BackButton.Click += BackButton_Click;
            llsExpenses.ItemsSource = MainPage.expensesList;
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
            this.PageHeader.BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            (Application.Current as App).SELECTED_EXPENSE = selectedExpense;
            this.Frame.Navigate(typeof(ExpenseDetail));

            llsExpenses.SelectedItem = null;
        }

        private void OnListViewLoaded(object sender, RoutedEventArgs e)
        {
            var listview = sender as ListViewBase;
            if (listview != null)
            {
                // Attach to the view changed event
                var _scrollViewer = listview.GetFirstDescendantOfType<ScrollViewer>();
                if (_scrollViewer != null)
                {
                    _scrollViewer.ViewChanged += OnViewChanged;
                }
            }
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var _scrollViewer = sender as ScrollViewer;
            // If scrollviewer is scrolled down at least 90%
            if (_scrollViewer.VerticalOffset > Math.Max(_scrollViewer.ScrollableHeight * 0.6, _scrollViewer.ScrollableHeight - 200))
            {
                if (MainPage.expenseLoadingBackgroundWorker.IsBusy != true && MainPage.morePages)
                {
                    MainPage.pageNo++;
                    MainPage.expenseLoadingBackgroundWorker.RunWorkerAsync(false);
                }
            }
        }
    }
}
