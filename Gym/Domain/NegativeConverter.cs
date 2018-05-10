using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gym.Domain
{

    [ValueConversion(typeof(bool), typeof(bool))]
    public class NegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            return int.TryParse(value.ToString().Replace(",", ""), out i) ? i * -1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            return int.TryParse(value.ToString().Replace(",", ""), out i) ? i * -1 : 0;
        }
    }
}
