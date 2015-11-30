using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    class NetBalances : INotifyPropertyChanged
    {
        private string netBalance;
        private string positiveBalance;
        private string negativeBalance;
        private string currencyCode;

        public void setBalances(string currency, double net, double positive, double negative)
        {
            currencyCode = currency.ToUpper() + " ";
            netBalance = Convert.ToDouble(net).ToString();
            positiveBalance = Convert.ToDouble(positive).ToString();
            negativeBalance = Convert.ToDouble(negative).ToString();
        }


        public String NetBalance
        {
            get { return netBalance; }
            set
            {
                netBalance = value;
                OnPropertyChanged("NetBalance");
            }
        }

        public String PositiveBalance
        {
            get { return positiveBalance; }
            set
            {
                positiveBalance = value;
                OnPropertyChanged("PositiveBalance");
            }
        }

        public String NegativeBalance
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
