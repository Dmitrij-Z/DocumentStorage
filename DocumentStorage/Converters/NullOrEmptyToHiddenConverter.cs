using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class NullOrEmptyToHiddenConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(!(value is string))
            {
                return Visibility.Hidden;
            }
            return string.IsNullOrEmpty(value as string) ? Visibility.Hidden : Visibility.Visible;
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
