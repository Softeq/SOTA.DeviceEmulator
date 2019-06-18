using System;
using System.Globalization;
using System.Windows.Data;

namespace SOTA.DeviceEmulator.ViewModels.Converters
{
    public class ConnectionStatusBooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isConnected = value != null && (bool) value;
            var textType = parameter != null
                ? Enum.Parse(typeof(ConnectionStatusTextType), parameter.ToString())
                : ConnectionStatusTextType.InfoLabel;

            switch (textType)
            {
                case ConnectionStatusTextType.InfoLabel:
                    return isConnected ? "Online" : "Offline";
                case ConnectionStatusTextType.ButtonLabel:
                    return isConnected ? "Disconnect" : "Connect";
                case ConnectionStatusTextType.Color:
                    return isConnected ? "Green" : "Red";
                default:
                    throw new ArgumentException("Unsupported connection status text type.", nameof(textType));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
