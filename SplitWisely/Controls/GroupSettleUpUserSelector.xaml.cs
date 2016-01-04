using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace SplitWisely.Controls
{
    public sealed partial class GroupSettleUpUserSelector : UserControl
    {
        Action<Debt_Group> Close;

        public GroupSettleUpUserSelector(List<Debt_Group> expenseUsers, Action<Debt_Group> close)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.Close = close;
        }

        private void llsFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;

            Close(llsFriends.SelectedItem as Debt_Group);
        }
    }
}
