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
        ObservableCollection<Group> groupsList;

        private object o = new object();

        public GroupsPage()
        {
            this.InitializeComponent();
            groupsList = new ObservableCollection<Group>();
            llsGroups.ItemsSource = groupsList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Task.Run(() =>
            {
                loadGroups();
            });
        }

        private async void loadGroups()
        {
            List<Group> allGroups;
            lock (o)
            {
                QueryDatabase obj = new QueryDatabase();
                allGroups = obj.getAllGroups();
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 groupsList.Clear();
                 if (allGroups != null)
                 {
                     foreach (var group in allGroups)
                     {
                         groupsList.Add(group);
                     }
                 }
             });
        }

    }
}
