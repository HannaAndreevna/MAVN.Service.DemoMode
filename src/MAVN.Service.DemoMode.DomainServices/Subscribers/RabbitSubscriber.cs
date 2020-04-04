using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace MAVN.Service.DemoMode.DomainServices.Subscribers
{
    public abstract class RabbitSubscriber<TMessage> : IStartStop
    {
        private RabbitMqSubscriber<TMessage> _subscriber;

        private readonly ILogFactory _logFactory;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private readonly string _contextName;

        protected readonly ILog Log;

        protected RabbitSubscriber(string connectionString, string exchangeName, ILogFactory logFactory)
        {
            Log = logFactory.CreateLog(this);
            _logFactory = logFactory;

            _connectionString = connectionString;
            _exchangeName = exchangeName;
            _contextName = GetType().Name;
        }

        public void Start()
        {
            var rabbitMqSubscriptionSettings = RabbitMqSubscriptionSettings.ForSubscriber(_connectionString,
                    _exchangeName,
                    "demoMode")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<TMessage>(
                    _logFactory,
                    rabbitMqSubscriptionSettings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        rabbitMqSubscriptionSettings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TMessage>())
                .Subscribe(StartProcessingAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        public void Dispose()
        {
            _subscriber.Dispose();
        }

        public abstract Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(TMessage message);

        private async Task StartProcessingAsync(TMessage message)
        {
            Log.Info($"{_contextName} event received", message);

            var result = await ProcessMessageAsync(message);

            if (!result.isSuccessful)
            {
                Log.Error(message: $"{_contextName} event was not processed", context: new
                {
                    Event = message,
                    ErrorMessage = result.errorMessage
                });

                return;
            }

            if (!string.IsNullOrEmpty(result.errorMessage))
            {
                Log.Info($"{_contextName} process message", result.errorMessage);
            }

            Log.Info($"{_contextName} event was processed", message);
        }
    }
}
