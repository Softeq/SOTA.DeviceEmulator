using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Caliburn.Micro;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;
using SOTA.DeviceEmulator.Services.Provisioning;
using SOTA.DeviceEmulator.ViewModels;

namespace SOTA.DeviceEmulator
{
    public class Bootstrapper : BootstrapperBase
    {
        private IHost _host;
        private ILogger _logger;
        private ObservableCollectionLogEventSink _uiLogSink;

        public Bootstrapper()
        {
            Initialize();
        }

        private ILifetimeScope Container => _host.Services.GetRequiredService<ILifetimeScope>();

        protected override void Configure()
        {
            var logCollection = new ObservableCollection<LogEventViewModel>();
            _uiLogSink = new ObservableCollectionLogEventSink(logCollection);
            _logger = new LoggerConfiguration()
                .UseSotaDeviceEmulatorConfiguration(_uiLogSink, LogEventLevel.Debug)
                .CreateLogger()
                .ForContext(GetType());
            var app = new DeviceEmulatorModule(_logger, logCollection);
            var hostBuilder = new HostBuilder();
            _host = hostBuilder
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(app))
                .UseSerilog(_logger, true)
                .Build();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
            // Background services should run on background thread
            Task.Run(() => _host.Start()).ContinueWith(OnHostStartupFailed, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void OnHostStartupFailed(Task startupTask)
        {
            // Should be non null as continuation is only invoked on failure
            // ReSharper disable once PossibleNullReferenceException
            _logger.Fatal(startupTask.Exception.GetBaseException(), "Background host startup failed.");
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _uiLogSink.IsEnabled = false;
            Task.Run(() => Container.Resolve<IMediator>().Send(new DisconnectCommand())).Wait();
            _host?.Dispose();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error(e.Exception, "Unhandled exception caught.");
            MessageBox.Show("Unexpected error happened. Application will be terminated.", "Unexpected Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        protected override object GetInstance(Type service, string key)
        {
            return key == null ? Container.Resolve(service) : Container.ResolveNamed(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(service);

            return Container.Resolve(type) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }
    }
}