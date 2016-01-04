using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitWisely.Converter.ExpandViewerConverters
{
    public class AllDebtAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string amount = "";
            ExpandableListModel expandableModel = value as ExpandableListModel;
            List<Debt_Group> allDebts = expandableModel.debtList;
            double finalBalance = Helpers.getUserGroupDebtAmount(allDebts, expandableModel.groupUser.id);

            //if final balance is 0, then anyways we are not shwoing the balance.
            if (finalBalance != 0)
            {
                string currency = allDebts[0].currency_code;
                amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
            }

            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
