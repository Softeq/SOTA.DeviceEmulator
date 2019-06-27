using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Services.Provisioning;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class ConnectionViewModel : Screen, ITabViewModel
    {
        private readonly IMediator _mediator;
        private readonly IConnectionOptions _connectionOptions;
        private bool _isConnected;
        private string _certificatePath;
        private string _selectedEnvironment;
        private string _errorMessage;
        private bool _isLoading;

        public ConnectionViewModel(IConnectionOptions connectionOptions, IMediator mediator) : this()
        {
            _mediator = Ensure.Any.IsNotNull(mediator, nameof(mediator));
            _connectionOptions = Ensure.Any.IsNotNull(connectionOptions, nameof(connectionOptions));
            _selectedEnvironment = _connectionOptions.DefaultEnvironment;
        }

        [DesignOnly(true)]
        private ConnectionViewModel()
        {
            DisplayName = "Connection";
            HeaderText = "Connection Settings";
        }

        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public string HeaderText { get; }

        public bool CanConnect => !string.IsNullOrEmpty(_certificatePath) && !IsLoading;
        public bool CanBrowseFiles => !_isConnected;
        public bool CanCreateCertificate => !_isConnected;

        public IReadOnlyCollection<string> Environments => _connectionOptions.Environments;

        public string SelectedEnvironment
        {
            get => _selectedEnvironment;
            set
            {
                Set(ref _selectedEnvironment, value, nameof(SelectedEnvironment));
                NotifyOfPropertyChange(nameof(DeviceProvisioningServiceIdScope));
            }
        }

        public string DeviceProvisioningServiceEndpoint => _connectionOptions.DeviceProvisioningServiceEndpoint;

        public string DeviceProvisioningServiceIdScope => _connectionOptions.GetDeviceProvisioningServiceIdScope(SelectedEnvironment);

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => CanBrowseFiles);
                NotifyOfPropertyChange(() => CanCreateCertificate);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                Set(ref _isLoading, value, nameof(IsLoading));
                NotifyOfPropertyChange(nameof(CanConnect));
            }
        }

        public string CertificatePath
        {
            get => _certificatePath;
            set
            {
                _certificatePath = value;
                NotifyOfPropertyChange(() => CertificatePath);
                NotifyOfPropertyChange(() => CanConnect);
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value, nameof(ErrorMessage));
        }

        public async Task Connect()
        {
            ErrorMessage = null;
            try
            {
                IsLoading = true;
                var command = IsConnected
                    ? (IRequest<ConnectionModel>)new DisconnectCommand()
                    : new ConnectCommand(
                        CertificatePath,
                        DeviceProvisioningServiceEndpoint,
                        DeviceProvisioningServiceIdScope
                    );
                var connection = await _mediator.Send(command);
                IsConnected = !IsConnected;
                var statusChanged = new ConnectionStatusChangedEventArgs(IsConnected, connection.DeviceDisplayName);
                OnConnectionStatusChanged(statusChanged);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void BrowseFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "device.pfx",
                DefaultExt = ".pfx",
                Filter = "Personal Information Exchange (.pfx)|*.pfx"
            };

            // Show open file dialog box
            var result = dialog.ShowDialog();

            if (result == true)
            {
                CertificatePath = dialog.FileName;
            }
        }

        public async Task CreateCertificate()
        {
            ErrorMessage = null;
            try
            {
                var command = new CreateCertificateCommand { Environment = SelectedEnvironment };
                var certificatePath = await _mediator.Send(command);
                CertificatePath = certificatePath;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            
        }

        private void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs e)
        {
            var eventHandler = ConnectionStatusChanged;
            eventHandler?.Invoke(this, e);
        }
    }
}