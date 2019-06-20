using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace SOTA.DeviceEmulator.Views.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = (Enum)value;
            var enumDescription = GetEnumDescription(enumValue);

            return enumDescription;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            return attributes.Length > 0
                ? attributes[0].Description
                : value.ToString();
        }
    }
}
