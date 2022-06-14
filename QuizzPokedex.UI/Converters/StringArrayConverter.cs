using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace QuizzPokedex.UI.Converters
{
    public class StringArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[] strings && parameter is string format)
            {
                try
                {
                    return string.Format(format, strings);
                }
                catch (Exception)
                {
                }
            }
            return string.Empty;
        }

        //Must implement this if Binding with Mode=TwoWay
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
