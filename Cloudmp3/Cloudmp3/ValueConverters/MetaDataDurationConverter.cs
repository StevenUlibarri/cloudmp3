using System;
using System.Windows.Data;

namespace Cloudmp3.ValueConverters
{
    public class MetaDataDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int milis = (int)value;
            TimeSpan span = TimeSpan.FromSeconds(milis/1000);
            return span.ToString(@"m\:s");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
