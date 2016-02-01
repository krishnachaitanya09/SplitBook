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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SplitBook.Controls
{
    public sealed partial class BackButton : UserControl
    {
        public event RoutedEventHandler Click;
       
        public BackButton()
        {
            this.InitializeComponent();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Click != null)
                this.Click(this, e);
        }
    }
}
