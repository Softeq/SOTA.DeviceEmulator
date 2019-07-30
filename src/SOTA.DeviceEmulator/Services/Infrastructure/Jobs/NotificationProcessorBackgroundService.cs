using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Autofac;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public class NotificationProcessorBackgroundService : BackgroundService, IHandle<INotification>
    {
        private readonly ILifetimeScope _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private ActionBlock<INotification> _processingBlock;
        private CancellationToken _stoppingToken;

        public NotificationProcessorBackgroundService(ILifetimeScope container)
        {
            _container = Ensure.Any.IsNotNull(container, nameof(container));
            _eventAggregator = container.Resolve<IEventAggregator>();
            _logger = container.Resolve<ILogger>().ForContext(GetType());
        }

        public void Handle(INotification message)
        {
            var success = _processingBlock.Post(message);
            var notificationType = message.GetType().Name;
            if (!success)
            {
                _logger.Error("Failed to enqueue {NotificationType} for processing. Queue size: {QueueSize}",
                    notificationType, _processingBlock.InputCount);
            }
            else
            {
                _logger.Verbose("Successfully queued {NotificationType} for processing. Queue size: {QueueSize}.",
                    notificationType, _processingBlock.InputCount);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _processingBlock = new ActionBlock<INotification>(ProcessAsync, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = stoppingToken
            });
            _logger.Information("Notification processor is started.");
            stoppingToken.Register(Stop);
            _eventAggregator.Subscribe(this);
            // ReSharper disable once InconsistentlySynchronizedField
            await _processingBlock.Completion;
            _logger.Information("Notification processor is stopped.");
        }

        private async Task ProcessAsync(INotification notification)
        {
            try
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var mediator = scope.Resolve<IMediator>();
                    await mediator.Publish(notification, _stoppingToken);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Unhandled exception caught while handling {NotificationType}.", notification.GetType().Name);
            }
        }

        private void Stop()
        {
            _eventAggregator.Unsubscribe(this);
            _processingBlock.Complete();
        }
    }
}