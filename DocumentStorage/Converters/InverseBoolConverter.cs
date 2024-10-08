using System;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class InverseBoolConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = false;

            switch ((bool)value)
            {
                case true: ReturnValue = false; break;
                case false: ReturnValue = true; break;
            }

            return ReturnValue;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = false;

            switch ((bool)value)
            {
                case false: ReturnValue = true; break;
                case true: ReturnValue = false; break;
            }

            return ReturnValue;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
