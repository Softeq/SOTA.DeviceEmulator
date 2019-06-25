using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using EnsureThat;
using Serilog.Core;
using Serilog.Events;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Logging
{
    public class ObservableCollectionLogEventSink : ILogEventSink
    {
        public ObservableCollectionLogEventSink(ObservableCollection<LogEventViewModel> logs)
        {
            Logs = Ensure.Any.IsNotNull(logs, nameof(logs));
            IsEnabled = true;
        }

        public ObservableCollection<LogEventViewModel> Logs { get; }

        public bool IsEnabled { get; set; }

        public void Emit(LogEvent logEvent)
        {
            if (!IsEnabled)
            {
                return;
            }
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(logEvent.RenderMessage());
            if (logEvent.Exception != null)
            {
                messageBuilder.AppendLine(logEvent.Exception.ToString());
            }
            var vm = new LogEventViewModel(logEvent.Timestamp, logEvent.Level, messageBuilder.ToString());
            Application.Current.Dispatcher.Invoke(() => Logs.Add(vm));
        }
    }
}