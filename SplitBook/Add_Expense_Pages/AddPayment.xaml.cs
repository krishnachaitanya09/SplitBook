using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using SplitBook.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace SplitBook.Add_Expense_Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPayment : Page
    {
        User paymentUser;
        public double TransferAmount { get; set; }
        public string Currency { get; set; }
        public string Details { get; set; }

        int paymentType;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public AddPayment()
        {
            this.InitializeComponent();
            BackButton.Tapped += BackButton_Tapped;

            paymentUser = (Application.Current as App).PAYMENT_USER;
            paymentType = (Application.Current as App).PAYMENT_TYPE;

            SetupData();

            if (paymentType == Constants.PAYMENT_TO)
            {
                toUser.DataContext = paymentUser;
                fromUser.DataContext = App.currentUser;
            }
            else
            {
                toUser.DataContext = App.currentUser;
                fromUser.DataContext = paymentUser;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("AddPaymentPage");
            base.OnNavigatedTo(e);
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void BtnOkay_Click(object sender, RoutedEventArgs e)
        {
            //to hide the keyboard if any
            this.Focus(FocusState.Programmatic);
            try
            {
                String cost;
                if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                    cost = tbAmount.Text.Replace(".", ",");
                else
                    cost = tbAmount.Text.Replace(",", ".");

                TransferAmount = Convert.ToDouble(cost);
            }
            catch (FormatException)
            {
                return;
            }
            Currency = tbCurrency.Text;
            Details = tbDetails.Text;

            if (TransferAmount != 0 && !String.IsNullOrEmpty(Currency))
            {
                busyIndicator.IsActive = true;

                await AddNewPayment();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void SetupData()
        {
            Balance_User defaultBalance = GetPaymentAmount();
            TransferAmount = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
            Currency = defaultBalance.currency_code;

            tbCurrency.Text = Currency;
            tbAmount.Text = String.Format("{0:0.00}", Math.Abs(TransferAmount));

            DateTime now = DateTime.UtcNow;
            string dateString = now.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            tbDate.Text = "on " + dateString;
        }

        private Balance_User GetPaymentAmount()
        {
            foreach (var balance in paymentUser.balance)
            {
                if (paymentType == Constants.PAYMENT_TO)
                {
                    if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) < 0)
                        return balance;
                }
                else
                {
                    if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) > 0)
                        return balance;
                }
            }
            return null;
        }

        private async Task AddNewPayment()
        {
            //only setup the needed details.
            Expense paymentExpense = new Expense()
            {
                payment = true,
                cost = TransferAmount.ToString(),
                currency_code = Currency,
                creation_method = "payment",
                description = "Payment",
                details = Details,
                group_id = (Application.Current as App).PAYMENT_GROUP,
                users = new List<Expense_Share>()
            };
            Expense_Share fromUser = new Expense_Share();
            Expense_Share toUser = new Expense_Share();
            if (paymentType == Constants.PAYMENT_TO)
            {
                fromUser.user_id = App.currentUser.id;
                fromUser.owed_share = "0";
                fromUser.paid_share = TransferAmount.ToString();

                toUser.user_id = paymentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = TransferAmount.ToString();
            }
            else
            {
                toUser.user_id = App.currentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = TransferAmount.ToString();

                fromUser.user_id = paymentUser.id;
                fromUser.paid_share = TransferAmount.ToString();
                fromUser.owed_share = "0";
            }

            paymentExpense.users.Add(fromUser);
            paymentExpense.users.Add(toUser);

            ModifyDatabase modify = new ModifyDatabase(_recordPaymentCompleted);
            await modify.AddExpense(paymentExpense);
        }

        private async void _recordPaymentCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;
                    this.Frame.Navigate(typeof(FriendsPage));
                    await MainPage.Current.FetchData();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.Logout();
                        (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Unable to record payment", "Error");
                        await messageDialog.ShowAsync();
                    }
                });
            }
        }

        private void TbAmount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep))
                e.Handled = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbAmount.Focus(FocusState.Programmatic);
            tbAmount.Select(tbAmount.Text.Length, 0);
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }
    }
}
