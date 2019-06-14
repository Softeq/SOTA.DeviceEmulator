using System.Collections.ObjectModel;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Infrastructure.Logging;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class LogViewModel : PropertyChangedBase
    {
        public LogViewModel(ObservableCollection<LogEventViewModel> logs)
        {
            Logs = Ensure.Any.IsNotNull(logs, nameof(logs));
        }

        public ObservableCollection<LogEventViewModel> Logs { get; }
    }
}