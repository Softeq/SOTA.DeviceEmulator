using System.Collections.ObjectModel;
using Autofac;
using Caliburn.Micro;
using EnsureThat;
using GeoAPI.Geometries;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;
using SOTA.DeviceEmulator.Services;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;
using SOTA.DeviceEmulator.Services.Infrastructure.ModelMetadata;
using SOTA.DeviceEmulator.Services.Infrastructure.Serialization;
using SOTA.DeviceEmulator.Services.Provisioning;

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
            builder.Register<ServiceFactory>(
                ctx =>
                {
                    var c = ctx.Resolve<IComponentContext>();
                    return t => c.Resolve(t);
                });

            builder.RegisterType<WindowManager>()
                .As<IWindowManager>()
                .SingleInstance();
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            // Please, add registrations from higher architectural levels to lower ones
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<PropertyChangedBase>()
                .AsSelf()
                .SingleInstance();

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
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .SingleInstance();
            builder
                .RegisterType<EventPublisher>()
                .As<IEventPublisher>()
                .SingleInstance();

            builder.RegisterType<ApplicationSettings>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
            builder.RegisterType<ApplicationContext>()
                   .As<IApplicationContext>()
                   .SingleInstance();
            builder.RegisterType<ModelMetadataProvider>()
                   .As<IModelMetadataProvider>()
                   .SingleInstance();
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            jsonSettings.Converters.Add(new StringEnumConverter { AllowIntegerValues = false });
            builder.RegisterInstance(jsonSettings);
            builder
                .RegisterType<TwinCollectionSerializer>()
                .As<ITwinCollectionSerializer>()
                .SingleInstance();
            builder
                .RegisterType<DevicePropertiesSerializer>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder
                .RegisterType<IoTHubMessageSerializer>()
                .As<IIoTHubMessageSerializer>()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(typeof(ISensor).Assembly)
                .AssignableTo<ISensor>()
                .As<ISensor>()
                .InstancePerDependency();
            builder
                .RegisterType<Device>()
                .As<IDevice>()
                .SingleInstance();
            builder
                .Register(ctx => ctx.Resolve<IDevice>().Configuration)
                .As<IDeviceConfiguration>()
                .InstancePerDependency();
            builder
                .RegisterAssemblyTypes(typeof(ITimeFunction<>).Assembly)
                .AssignableTo<ITimeFunction<double>>()
                .As<ITimeFunction<double>>()
                .SingleInstance();
            builder
                .RegisterAssemblyTypes(typeof(ITimeFunction<>).Assembly)
                .AssignableTo<ITimeFunction<IPoint>>()
                .As<ITimeFunction<IPoint>>()
                .InstancePerDependency();
        }
    }
}