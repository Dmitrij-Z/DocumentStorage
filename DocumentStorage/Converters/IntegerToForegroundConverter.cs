using System;
using System.Windows.Media;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class IntegerToForegroundConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                switch ((int)value)
                {
                    case 0: return Brushes.White;
                    case 1: return (SolidColorBrush)new BrushConverter().ConvertFrom("#0090f0"); //Brushes.DeepSkyBlue;
                    case 2: return Brushes.LimeGreen;
                }
                return Brushes.Black;
            }
            catch
            {
                return Brushes.Black;
            }
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
