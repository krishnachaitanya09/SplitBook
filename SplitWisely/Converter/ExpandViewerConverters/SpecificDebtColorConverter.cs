using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SplitWisely.Converter.ExpandViewerConverters
{
    public class SpecificDebtColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush colorBrush;
            Debt_Group specificDebt = value as Debt_Group;
            double finalBalance = System.Convert.ToDouble(specificDebt.amount, System.Globalization.CultureInfo.InvariantCulture);

            if (specificDebt.ownerId == specificDebt.from)
                colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;
            else
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
