using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitWisely.Converter
{
    public class ExpenseAmountInCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Expense expense = value as Expense;
            return expense.currency_code + String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(expense.cost, System.Globalization.CultureInfo.InvariantCulture)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
