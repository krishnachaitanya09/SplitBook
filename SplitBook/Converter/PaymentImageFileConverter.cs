using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace SplitBook.Converter
{
    /// <summary>
/// Caches the image that gets downloaded as part of Image control Source property.
/// </summary>
    public class PaymentImageFileConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Uri imageFileUri;
            bool payment = (bool)value;
            if (!payment)
            {
                imageFileUri = new Uri("ms-appx:///Assets/Images/expense_general.png");
            }
            else
            {
                imageFileUri = new Uri("ms-appx:///Assets/Images/expense_payment.png");
            }
            
            BitmapImage bm = new BitmapImage(imageFileUri);
            return bm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
