using System.Windows.Controls;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Framework;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly NavigationServiceHolder _navigationServiceHolder;

        public ShellViewModel(NavigationServiceHolder navigationServiceHolder)
        {
            _navigationServiceHolder = Ensure.Any.IsNotNull(navigationServiceHolder, nameof(navigationServiceHolder));
        }

        public void RegisterFrame(Frame frame)
        {
            var navigationService = new FrameAdapter(frame);
            // Adds INavigationService to IoC container, so that it's accessible from VMs
            _navigationServiceHolder.NavigationService = navigationService;
            navigationService.NavigateToViewModel(typeof(LayoutViewModel));
        }
    }
}