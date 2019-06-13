using Caliburn.Micro;
using Serilog;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class SensorsViewModel : Screen, ITabViewModel
    {
        public SensorsViewModel(ILogger logger)
        {
            DisplayName = "Sensors";
            logger.Debug("Sensors view model created.");
        }
    }
}