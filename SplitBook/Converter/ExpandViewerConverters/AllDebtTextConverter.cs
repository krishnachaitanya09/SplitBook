using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitBook.Converter.ExpandViewerConverters
{
    public class AllDebtTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text = "";
            ExpandableListModel expandableModel = value as ExpandableListModel;
            List<Debt_Group> allDebts = expandableModel.debtList;
            double finalBalance = Helpers.GetUserGroupDebtAmount(allDebts, expandableModel.groupUser.id);

            //if final balance is 0, then anyways we are not shwoing the balance.
            if (finalBalance == 0)
                text = "settled up";
            if (finalBalance > 0)
                text = "is owed";
            else if (finalBalance < 0)
                text = "owes";

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
