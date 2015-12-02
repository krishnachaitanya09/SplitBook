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
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitWisely.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivityPage : Page
    {
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private object o = new object();

        private int pageNo = 0;
        private bool morePages = true;

        public ActivityPage()
        {
            this.InitializeComponent();
            llsExpenses.ItemsSource = expensesList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Task.Run(async () =>
            {
                await loadExpenses();
            });
        }

        private async Task loadExpenses()
        {
            List<Expense> allExpenses;
            lock (o)
            {
                QueryDatabase obj = new QueryDatabase();
                allExpenses = obj.getAllExpenses(pageNo);
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (pageNo == 0)
                    expensesList.Clear();

                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;
                else
                    morePages = true;

                if (allExpenses != null)
                {
                    foreach (var expense in allExpenses)
                    {
                        expensesList.Add(expense);
                    }
                }
            });
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
            if (_scrollViewer.VerticalOffset > Math.Max(_scrollViewer.ScrollableHeight * 0.8, _scrollViewer.ScrollableHeight - 200))
            {
                if (morePages)
                {
                    pageNo++;
                    Task.Run(async () =>
                    {
                        await loadExpenses();
                    });
                }
            }
        }
    }
}
