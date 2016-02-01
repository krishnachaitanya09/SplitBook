using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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


namespace SplitBook.Controls
{
    public sealed partial class MultiplePayeeInputPopUpControl : UserControl
    {
        Action Close;
        decimal ExpenseCost;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public MultiplePayeeInputPopUpControl(ref ObservableCollection<Expense_Share> expenseUsers, Action close, decimal total)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.ExpenseCost = total;
            this.Close = close;
        }

        private void Okay_Tap(object sender, TappedRoutedEventArgs e)
        {
            if (calculateTotalInput())
                Close();
        }

        private bool calculateTotalInput()
        {
            ObservableCollection<Expense_Share> expenseUsers = llsFriends.ItemsSource as ObservableCollection<Expense_Share>;
            decimal total = 0;
            for (int i = 0; i < expenseUsers.Count; i++)
            {
                if (!String.IsNullOrEmpty(expenseUsers[i].paid_share))
                {
                    if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                        expenseUsers[i].paid_share = expenseUsers[i].paid_share.Replace(".", ",");
                    else
                        expenseUsers[i].paid_share = expenseUsers[i].paid_share.Replace(",", ".");
                    total += Convert.ToDecimal(expenseUsers[i].paid_share);
                }
            }

            if (ExpenseCost == total)
                return true;

            else
            {
                tbError.Text = "The paid amount for each person do not add up to the total cost of the bill.";
                tbSum.Text = "Total: " + total + "/" + ExpenseCost;
                return false;
            }
        }

        private void tbAmount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep))
                e.Handled = true;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
