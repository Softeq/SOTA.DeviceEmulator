using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class ShellViewModel : Screen
    {
        public ShellViewModel()
        {
            var detailedVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                   ?.InformationalVersion ?? "0.0.0-local";
            var displayVersion = new string(detailedVersion.TakeWhile(c => c != '+').ToArray());
            Title = $"SOTA Device Emulator (v{displayVersion})";
        }

        public string Title { get; }

        public void RegisterFrame(Frame frame)
        {
            var navigationService = new FrameAdapter(frame);
            navigationService.NavigateToViewModel(typeof(LayoutViewModel));
        }
    }
}