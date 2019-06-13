using System.Collections.Generic;
using Caliburn.Micro;
using EnsureThat;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class LayoutViewModel : Conductor<ITabViewModel>.Collection.OneActive
    {
        public LayoutViewModel(IEnumerable<ITabViewModel> tabs, StatusBarViewModel statusBarViewModel,
            LogViewModel logViewModel)
        {
            Log = Ensure.Any.IsNotNull(logViewModel, nameof(logViewModel));
            StatusBar = Ensure.Any.IsNotNull(statusBarViewModel, nameof(statusBarViewModel));
            Ensure.Any.IsNotNull(tabs, nameof(tabs));

            DeviceDisplayName = "Device name will be here.";
            Items.AddRange(tabs);
            StatusBar = statusBarViewModel;
        }

        public string DeviceDisplayName { get; }

        public StatusBarViewModel StatusBar { get; }

        public LogViewModel Log { get; }
    }
}