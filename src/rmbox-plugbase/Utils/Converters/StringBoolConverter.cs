using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Ruminoid.Toolbox.Utils.Converters
{
    public class StringBoolConverter : IValueConverter
    {
        internal StringBoolConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string stringValue) ||
                !(parameter is string stringParameter))
                return false;

            return stringValue == stringParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue) ||
                !(parameter is string stringParameter))
                return BindingOperations.DoNothing;

            return boolValue ? stringParameter : BindingOperations.DoNothing;
        }
    }
}
