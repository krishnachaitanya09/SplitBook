using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitBook.Converter.ExpandViewerConverters
{
    public class SpecificDebtAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string amount = "";
            Debt_Group specificDebt = value as Debt_Group;
            double finalBalance = System.Convert.ToDouble(specificDebt.amount, System.Globalization.CultureInfo.InvariantCulture);

            string currency = specificDebt.currency_code;
            amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));

            if (specificDebt.ownerId == specificDebt.from)
                return amount + " to " + specificDebt.toUser.name;
            else
                return amount + " by " + specificDebt.fromUser.name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
