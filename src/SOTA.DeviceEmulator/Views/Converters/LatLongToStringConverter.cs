using System;
using System.Globalization;
using System.Windows.Data;

namespace SOTA.DeviceEmulator.Views.Converters
{
    public class LatLongToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var coordinateValue = (double?) value ?? 0;

            return $"{coordinateValue:0.00000}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
