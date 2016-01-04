using SplitWisely.Model;
using SplitWisely.Utilities;
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
    public class AllDebtColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush colorBrush;
            ExpandableListModel expandableModel = value as ExpandableListModel;
            List<Debt_Group> allDebts = expandableModel.debtList;
            double finalBalance = Helpers.getUserGroupDebtAmount(allDebts, expandableModel.groupUser.id);

            if (finalBalance > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            else if (finalBalance == 0)
                colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;
            else
                colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
