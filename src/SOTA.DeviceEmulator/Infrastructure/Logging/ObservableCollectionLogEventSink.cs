using System.Collections.ObjectModel;
using EnsureThat;
using Serilog.Core;
using Serilog.Events;

namespace SOTA.DeviceEmulator.Framework
{
    public class ObservableCollectionLogEventSink : ILogEventSink
    {
        public ObservableCollectionLogEventSink(ObservableCollection<LogEventViewModel> logs)
        {
            Logs = Ensure.Any.IsNotNull(logs, nameof(logs));
        }

        public ObservableCollection<LogEventViewModel> Logs { get; }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            var vm = new LogEventViewModel(logEvent.Timestamp, logEvent.Level, message);
            Logs.Add(vm);
        }
    }
}