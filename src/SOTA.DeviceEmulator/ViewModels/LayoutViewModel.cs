using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Services.Provisioning;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class LayoutViewModel : Conductor<ITabViewModel>.Collection.OneActive
    {
        private readonly IMediator _mediator;
        private string _deviceDisplayName;

        public LayoutViewModel(
            IEnumerable<ITabViewModel> tabs,
            StatusBarViewModel statusBarViewModel,
            LogViewModel logViewModel,
            IMediator mediator
        )
        {
            _mediator = Ensure.Any.IsNotNull(mediator, nameof(mediator));
            Log = Ensure.Any.IsNotNull(logViewModel, nameof(logViewModel));
            StatusBar = Ensure.Any.IsNotNull(statusBarViewModel, nameof(statusBarViewModel));
            Ensure.Any.IsNotNull(tabs, nameof(tabs));

            var tabViewModels = tabs as ITabViewModel[] ?? tabs.ToArray();
            Items.AddRange(tabViewModels);
            StatusBar = statusBarViewModel;

            var connectionViewModel = tabViewModels.OfType<ConnectionViewModel>().FirstOrDefault();
            Ensure.Any.IsNotNull(connectionViewModel, nameof(connectionViewModel));

            connectionViewModel.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        public string DeviceDisplayName
        {
            get => _deviceDisplayName;
            set => Set(ref _deviceDisplayName, value, nameof(DeviceDisplayName));
        }

        public StatusBarViewModel StatusBar { get; }

        public LogViewModel Log { get; }

        protected override async void OnInitialize()
        {
            base.OnInitialize();
            var connection = await _mediator.Send(new GetConnectionQuery());
            DeviceDisplayName = connection.DeviceDisplayName;
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            StatusBar.IsConnected = e.IsConnected;
            DeviceDisplayName = e.DeviceDisplayName;
        }
    }
}