using System.Collections.ObjectModel;
using Autofac;
using Caliburn.Micro;
using Serilog;
using Serilog.Events;
using SOTA.DeviceEmulator.Infrastructure.Logging;
using SOTA.DeviceEmulator.ViewModels;

namespace SOTA.DeviceEmulator
{
    public class DeviceEmulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Infrastructure
            var logsCollection = new ObservableCollection<LogEventViewModel>();
            var logger = new LoggerConfiguration()
                .UseSotaDeviceEmulatorConfiguration(logsCollection, LogEventLevel.Debug)
                .CreateLogger();
            builder.RegisterInstance(logsCollection);
            builder.RegisterInstance(logger).As<ILogger>();

            // Please, add registrations from higher architectural levels to lower ones
            builder.RegisterType<WindowManager>()
                .As<IWindowManager>()
                .SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<Screen>()
                .Where(t => !typeof(ITabViewModel).IsAssignableFrom(t))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<ITabViewModel>()
                .AsSelf()
                .As<ITabViewModel>()
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<PropertyChangedBase>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}