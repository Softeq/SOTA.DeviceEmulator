using System.Windows.Controls;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class ShellViewModel : Screen
    {
        public ShellViewModel()
        {
        }

        public void RegisterFrame(Frame frame)
        {
            var navigationService = new FrameAdapter(frame);
            navigationService.NavigateToViewModel(typeof(LayoutViewModel));
        }
    }
}