using System;
using EnsureThat;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.ViewModels;

namespace SOTA.DeviceEmulator.Services
{
    public class DeviceState : IDeviceState
    {
        private readonly SensorsViewModel _sensorsViewModel;
        private readonly StatusBarViewModel _statusBarViewModel;

        public DeviceState(SensorsViewModel sensorsViewModel, StatusBarViewModel statusBarViewModel)
        {
            _sensorsViewModel = Ensure.Any.IsNotNull(sensorsViewModel, nameof(sensorsViewModel));
            _statusBarViewModel = Ensure.Any.IsNotNull(statusBarViewModel, nameof(statusBarViewModel));
        }

        public IPulseSensorOptions PulseSensorOptions => _sensorsViewModel;
        public ILocationSensorOptions LocationSensorOptions => _sensorsViewModel;

        public bool IsTransmissionEnabled
        {
            get => _statusBarViewModel.IsTransmissionEnabled;
            set => _statusBarViewModel.IsTransmissionEnabled = value;
        }

        public int TransmissionPeriod
        {
            get => _statusBarViewModel.TransmissionPeriod;
            set => _statusBarViewModel.TransmissionPeriod = value;
        }

        public TimeSpan SessionTime
        {
            get => _statusBarViewModel.SessionTime;
            set => _statusBarViewModel.SessionTime = value;
        }
    }
}
