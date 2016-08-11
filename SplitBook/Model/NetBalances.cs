using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    public class NetBalances : INotifyPropertyChanged
    {
        private string netBalance;
        private string positiveBalance;
        private string negativeBalance;
        private string currencyCode;

        public NetBalances()
        {
            NetBalance = "0";
            PositiveBalance = "0";
            NegativeBalance = "0";
        }

        public void setBalances(string currency_name, double net, double positive, double negative)
        {
            using (SplitBookContext db = new SplitBookContext())
            {
                Currency currency = db.Currency.Where(c => c.currency_code == currency_name.ToUpper()).FirstOrDefault();
                currencyCode = currency != null ? currency.unit : string.Empty;
            }
            if (currencyCode != String.Empty)
            {
                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                format.CurrencySymbol = currencyCode;
                format.CurrencyNegativePattern = 1;
                NetBalance = String.Format(format, "{0:C}", Convert.ToDouble(net));
                PositiveBalance = String.Format(format, "{0:C}", Convert.ToDouble(positive));
                NegativeBalance = String.Format(format, "{0:C}", Convert.ToDouble(negative));
            }
            else
            {
                NetBalance = Convert.ToDouble(net).ToString();
                PositiveBalance = Convert.ToDouble(positive).ToString();
                NegativeBalance =  Convert.ToDouble(negative).ToString();
            }
        }


        public string NetBalance
        {
            get { return netBalance; }
            set
            {
                netBalance = value;
                OnPropertyChanged("NetBalance");
            }
        }

        public string PositiveBalance
        {
            get { return positiveBalance; }
            set
            {
                positiveBalance = value;
                OnPropertyChanged("PositiveBalance");
            }
        }

        public string NegativeBalance
        {
            get { return negativeBalance; }
            set
            {
                negativeBalance = value;
                OnPropertyChanged("NegativeBalance");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
