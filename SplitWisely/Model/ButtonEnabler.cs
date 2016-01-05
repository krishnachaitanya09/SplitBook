using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class ButtonEnabler : INotifyPropertyChanged
    {
        private bool addButtonEnabled;
        private bool refreshButtonEnabled;
        private bool searchButtonEnabled;

        public bool AddButtonEnabled
        {
            get { return addButtonEnabled; }
            set
            {
                addButtonEnabled = value;
                OnPropertyChanged("AddButtonEnabled");
            }
        }

        public bool RefreshButtonEnabled
        {
            get { return refreshButtonEnabled; }
            set
            {
                refreshButtonEnabled = value;
                OnPropertyChanged("RefreshButtonEnabled");
            }
        }

        public bool SearchButtonEnabled
        {
            get { return searchButtonEnabled; }
            set
            {
                searchButtonEnabled = value;
                OnPropertyChanged("SearchButtonEnabled");
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
