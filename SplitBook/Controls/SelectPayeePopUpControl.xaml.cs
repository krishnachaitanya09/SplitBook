using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SplitBook.Controls
{
    public sealed partial class SelectPayeePopUpControl : UserControl
    {
        Action<Expense_Share, bool> Close;

        public SelectPayeePopUpControl(ObservableCollection<Expense_Share> expenseUsers, Action<Expense_Share, bool> close)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.Close = close;
        }

        private void llsFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;

            Close(llsFriends.SelectedItem as Expense_Share, false);
        }

        private void tbMultiplePayers_Tap(object sender, TappedRoutedEventArgs e)
        {
            Close(null, true);
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
