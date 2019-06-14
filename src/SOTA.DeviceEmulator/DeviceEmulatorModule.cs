using System.Collections.ObjectModel;
using Autofac;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Infrastructure;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;
using SOTA.DeviceEmulator.ViewModels;

namespace SOTA.DeviceEmulator
{
    public class DeviceEmulatorModule : Module
    {
        private readonly ILogger _logger;
        private readonly ObservableCollection<LogEventViewModel> _logCollection;

        public DeviceEmulatorModule(ILogger logger, ObservableCollection<LogEventViewModel> logCollection)
        {
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
            _logCollection = Ensure.Any.IsNotNull(logCollection, nameof(logCollection));
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Infrastructure
            builder.RegisterInstance(SystemClock.Instance).As<IClock>();

            builder.RegisterInstance(_logCollection);
            builder.RegisterInstance(_logger).As<ILogger>();

            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces();
            builder.Register(ToggleableMediatorServiceFactory.Create);

            builder.RegisterType<WindowManager>()
                .As<IWindowManager>()
                .SingleInstance();
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            // Please, add registrations from higher architectural levels to lower ones
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

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IHostedService>()
                .As<IHostedService>()
                .SingleInstance();
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<INotificationTimerRule>()
                .As<INotificationTimerRule>()
                .SingleInstance();
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .SingleInstance();
        }
    }
}