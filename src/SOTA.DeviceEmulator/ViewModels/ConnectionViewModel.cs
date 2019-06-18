using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class ConnectionViewModel : Screen, ITabViewModel
    {
        private bool _isConnected;
        private string _certificatePath;

        public ConnectionViewModel()
        {
            DisplayName = "Connection";
            HeaderText = "Connection Settings";
        }

        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public string HeaderText { get; }

        public bool IsConnectAllowed => !string.IsNullOrEmpty(_certificatePath);
        public bool IsChangeCertificateAllowed => !_isConnected;


        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsChangeCertificateAllowed);
            }
        }

        public string CertificatePath
        {
            get => _certificatePath;
            set
            {
                _certificatePath = value;
                NotifyOfPropertyChange(() => CertificatePath);
                NotifyOfPropertyChange(() => IsConnectAllowed);
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
                MessageBox.Show(e.Message, "Error");
            }
        }

        public void BrowseFiles()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
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