using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class BoolToCollapsedConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility ReturnValue = Visibility.Collapsed;

            switch ((bool)value)
            {
                case true: ReturnValue = Visibility.Visible; break;
                case false: ReturnValue = Visibility.Collapsed; break;
            }

            return ReturnValue;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = false;

            switch ((Visibility)value)
            {
                case Visibility.Visible: ReturnValue = true; break;
                case Visibility.Collapsed: ReturnValue = false; break;
            }

            return ReturnValue;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
