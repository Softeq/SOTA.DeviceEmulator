using System.Collections.Generic;
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
            StatusBarViewModel statusBarViewModel,
            LogViewModel logViewModel,
            ConnectionViewModel connectionViewModel,
            SensorsViewModel sensorsViewModel,
            IMediator mediator
        )
        {
            Ensure.Any.IsNotNull(connectionViewModel, nameof(connectionViewModel));

            _mediator = Ensure.Any.IsNotNull(mediator, nameof(mediator));
            Log = Ensure.Any.IsNotNull(logViewModel, nameof(logViewModel));
            StatusBar = Ensure.Any.IsNotNull(statusBarViewModel, nameof(statusBarViewModel));

            var tabViewModels = new List<ITabViewModel>
            {
                connectionViewModel,
                sensorsViewModel
            };
            Items.AddRange(tabViewModels);
            StatusBar = statusBarViewModel;

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