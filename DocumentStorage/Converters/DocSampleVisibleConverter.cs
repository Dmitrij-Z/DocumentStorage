using System;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class DocSampleVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool editMode = (bool)values[0];
            bool docExists = values[1] != null;

            return editMode || docExists ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
