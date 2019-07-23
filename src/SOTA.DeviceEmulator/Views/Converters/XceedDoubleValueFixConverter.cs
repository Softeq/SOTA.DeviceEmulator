using System;
using System.Globalization;
using System.Windows.Data;

namespace SOTA.DeviceEmulator.Views.Converters
{
    /// <summary>
    /// This converter is required due to a bug with xceed double up down input controls.
    /// For some reason after several changes these controls provide value like 12,09999999999989 instead of 12,1.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class XceedDoubleValueFixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !double.TryParse(value.ToString(), out var valueToConvert))
            {
                return value;
            }

            var roundedValue = Math.Round(valueToConvert, 1);
            return roundedValue;
        }
    }
}
