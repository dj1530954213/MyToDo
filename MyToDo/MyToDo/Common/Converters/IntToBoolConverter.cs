using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Xml.XPath;

namespace MyToDo.Common.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                if (result == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null&&bool.TryParse(value.ToString(),out bool result))
            {
                if (result == true)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
