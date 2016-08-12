using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SplitBook.Converter
{
    public class ExpenseAmountInCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Expense expense = value as Expense;
            if (expense.currency_code.Equals(App.currentUser.default_currency))
            {
                QueryDatabase obj = new QueryDatabase();
                string unit = obj.getUnitForCurrency(expense.currency_code);
                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                format.CurrencySymbol = unit;
                format.CurrencyNegativePattern = 1;
                return String.Format(format, "{0:C}", Math.Abs(System.Convert.ToDouble(expense.cost, CultureInfo.InvariantCulture)));
            }
            else
            {
                return expense.currency_code + String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(expense.cost, CultureInfo.InvariantCulture)));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
