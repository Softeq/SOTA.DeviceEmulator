using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Services.Provisioning;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class ConnectionViewModel : Screen, ITabViewModel
    {
        private readonly IConnectionOptions _connectionOptions;
        private bool _isConnected;
        private string _certificatePath;
        private string _selectedEnvironment;

        public ConnectionViewModel(IConnectionOptions connectionOptions)
        {
            _connectionOptions = Ensure.Any.IsNotNull(connectionOptions, nameof(connectionOptions));
            _selectedEnvironment = _connectionOptions.DefaultEnvironment;
            DisplayName = "Connection";
            HeaderText = "Connection Settings";
        }

        [DesignOnly(true)]
        private ConnectionViewModel()
        {
        }

        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public string HeaderText { get; }

        public bool CanConnect => !string.IsNullOrEmpty(_certificatePath);
        public bool CanBrowseFiles => !_isConnected;

        public IReadOnlyCollection<string> Environments => _connectionOptions.Environments;

        public string SelectedEnvironment
        {
            get => _selectedEnvironment;
            set => Set(ref _selectedEnvironment, value, nameof(SelectedEnvironment));
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => CanBrowseFiles);
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

        public async Task Connect()
        {
            try
            {
                await Task.Delay(1000);

                var networkError = (DateTime.Now.Second % 2) == 1;
                if (networkError)
                {
                    throw new Exception("Network Error!");
                }

                IsConnected = !IsConnected;
                OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs { IsConnected = IsConnected });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Connection Error");
            }
        }

        public void BrowseFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Certificate",
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

        private void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs e)
        {
            var eventHandler = ConnectionStatusChanged;
            eventHandler?.Invoke(this, e);
        }
    }
}