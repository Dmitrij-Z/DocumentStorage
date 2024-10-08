using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class IntegerToHiddenConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Hidden;
            }
            if ((int)value > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
