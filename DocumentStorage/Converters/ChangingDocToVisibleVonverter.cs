using DocumentStorage.Documents;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Converters
{
    class ChangingDocToVisibleVonverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values.Length != 7 || values[0] == null || values[0].GetType() != typeof(Doc))
                {
                    return Visibility.Hidden;
                }

                Doc currDoc = (Doc)values[0];

                string title = (string)values[1];
                string fileName = GetFileName(values[2], values[3]);
                string comment = (string)values[4];
                byte[] docData = (byte[])values[5];
                byte[] docSampleData = (byte[])values[6];

                if(CompareStringValues(currDoc.Title, title) &&
                    CompareStringValues(currDoc.Comment, comment) &&
                    CompareStringValues(currDoc.FileName, fileName) &&
                    currDoc.DocData.SequenceEqual(docData) &&
                    currDoc.DocSampleData.SequenceEqual(docSampleData))
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Если имя файла бланка пустое - возвращает значение имя файла образца
        /// </summary>
        private string GetFileName(object fileName, object sampleFileName)
        {
            if(string.IsNullOrEmpty((string)fileName))
            {
                return (string)sampleFileName;
            }
            return (string)fileName;
        }

        /// <summary>
        /// Возвращает результат сравнения при условии null и string.Empty равны
        /// </summary>
        private bool CompareStringValues(string val1, string val2)
        {
            return (val1 ?? string.Empty) == (val2 ?? string.Empty);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
