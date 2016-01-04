using SplitWisely.Controller;
using SplitWisely.Model;
using SplitWisely.Utilities;
using SplitWisely.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Navigation;


namespace SplitWisely.Add_Expense_Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPayment : Page
    {
        User paymentUser;
        BackgroundWorker addPaymentBackgroundWorker;
        public double transferAmount { get; set; }
        public string currency { get; set; }
        public string details { get; set; }

        int paymentType;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public AddPayment()
        {
            this.InitializeComponent();

            paymentUser = (Application.Current as App).PAYMENT_USER;
            paymentType = (Application.Current as App).PAYMENT_TYPE;

            addPaymentBackgroundWorker = new BackgroundWorker();
            addPaymentBackgroundWorker.WorkerSupportsCancellation = true;
            addPaymentBackgroundWorker.DoWork += new DoWorkEventHandler(addPaymentBackgroundWorker_DoWork);

            setupData();

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

        private void btnOkay_Click(object sender, RoutedEventArgs e)
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

                transferAmount = Convert.ToDouble(cost);
            }
            catch (FormatException)
            {
                return;
            }
            currency = tbCurrency.Text;
            details = tbDetails.Text;

            if (addPaymentBackgroundWorker.IsBusy != true && transferAmount != 0 && !String.IsNullOrEmpty(currency))
            {
                busyIndicator.IsActive = true;

                addPaymentBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void setupData()
        {
            Balance_User defaultBalance = getPaymentAmount();
            transferAmount = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
            currency = defaultBalance.currency_code;

            tbCurrency.Text = currency;
            tbAmount.Text = String.Format("{0:0.00}", Math.Abs(transferAmount));

            DateTime now = DateTime.UtcNow;
            string dateString = now.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            tbDate.Text = "on " + dateString;
        }

        private Balance_User getPaymentAmount()
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

        private void addPaymentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //only setup the needed details.
            Expense paymentExpense = new Expense();
            paymentExpense.payment = true;
            paymentExpense.cost = transferAmount.ToString();
            paymentExpense.currency_code = currency;
            paymentExpense.creation_method = "payment";
            paymentExpense.description = "Payment";
            paymentExpense.details = details;
            paymentExpense.group_id =  (Application.Current as App).PAYMENT_GROUP;
            paymentExpense.users = new List<Expense_Share>();

            Expense_Share fromUser = new Expense_Share();
            Expense_Share toUser = new Expense_Share();
            if (paymentType == Constants.PAYMENT_TO)
            {
                fromUser.user_id = App.currentUser.id;
                fromUser.owed_share = "0";
                fromUser.paid_share = transferAmount.ToString();

                toUser.user_id = paymentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = transferAmount.ToString();
            }
            else
            {
                toUser.user_id = App.currentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = transferAmount.ToString();

                fromUser.user_id = paymentUser.id;
                fromUser.paid_share = transferAmount.ToString();
                fromUser.owed_share = "0";
            }

            paymentExpense.users.Add(fromUser);
            paymentExpense.users.Add(toUser);

            ModifyDatabase modify = new ModifyDatabase(_recordPaymentCompleted);
            modify.addExpense(paymentExpense);
        }

        private async void _recordPaymentCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    busyIndicator.IsActive = false;
                    this.Frame.Navigate(typeof(MainPage));
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.logout();
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


        private void tbAmount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep))
                e.Handled = true;
        }
    }
}
