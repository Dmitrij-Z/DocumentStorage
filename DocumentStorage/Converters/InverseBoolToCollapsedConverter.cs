using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class InverseBoolToCollapsedConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility ReturnValue = Visibility.Visible;

            switch ((bool)value)
            {
                case true: ReturnValue = Visibility.Collapsed; break;
                case false: ReturnValue = Visibility.Visible; break;
            }

            return ReturnValue;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = true;

            switch ((Visibility)value)
            {
                case Visibility.Visible: ReturnValue = false; break;
                case Visibility.Collapsed: ReturnValue = true; break;
            }

            return ReturnValue;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
