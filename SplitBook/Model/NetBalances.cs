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

        public void setBalances(double net, double positive, double negative)
        {
            NetBalance = Convert.ToDouble(net).ToString();
            PositiveBalance = Convert.ToDouble(positive).ToString();
            NegativeBalance = Convert.ToDouble(negative).ToString();            
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
