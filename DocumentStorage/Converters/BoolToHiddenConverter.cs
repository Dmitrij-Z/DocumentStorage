using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class BoolToHiddenConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility ReturnValue = Visibility.Hidden;

            switch ((bool)value)
            {
                case true: ReturnValue = Visibility.Visible; break;
                case false: ReturnValue = Visibility.Hidden; break;
            }

            return ReturnValue;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = false;

            switch ((Visibility)value)
            {
                case Visibility.Visible: ReturnValue = true; break;
                case Visibility.Hidden: ReturnValue = false; break;
            }

            return ReturnValue;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
