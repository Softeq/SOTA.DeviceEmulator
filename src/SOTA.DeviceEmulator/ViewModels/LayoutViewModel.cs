using System.Collections.Generic;
using System.Linq;
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

            var tabViewModels = tabs as ITabViewModel[] ?? tabs.ToArray();

            DeviceDisplayName = "Device name will be here.";
            Items.AddRange(tabViewModels);
            StatusBar = statusBarViewModel;

            var connectionViewModel = (ConnectionViewModel)tabViewModels.FirstOrDefault(i => i is ConnectionViewModel);
            Ensure.Any.IsNotNull(connectionViewModel, nameof(connectionViewModel));

            connectionViewModel.ConnectionStatusChanged += statusBarViewModel.ToggleConnectionStatus;
        }

        

        public string DeviceDisplayName { get; }

        public StatusBarViewModel StatusBar { get; }

        public LogViewModel Log { get; }
    }
}