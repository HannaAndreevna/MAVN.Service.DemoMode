using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CustomerManagement.Contract.Events;
using MAVN.Service.DemoMode.DomainServices.Subscribers;
using MAVN.Service.DemoMode.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.DemoMode.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private readonly RabbitMqSettings _settings;

        private const string EmailCodeVerifiedExchangeName = "lykke.customer.emailcodeverified";
        private const string CustomerRegisteredExchangeName = "lykke.customer.registration";
        private const string SessionEndedExchangeName = "lykke.customer.session-end";
        private const string PartnersPaymentTokensReservedExchangeName = "lykke.wallet.partnerspaymenttokensreserved";

        public RabbitMqModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue.DemoModeService.RabbitMq;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Publishers
            builder.RegisterJsonRabbitPublisher<EmailCodeVerifiedEvent>(
                _settings.UserManagementConnString,
                EmailCodeVerifiedExchangeName);

            // Subscribers
            builder.RegisterType<CustomerRegistrationSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.UserManagementConnString)
                .WithParameter("exchangeName", CustomerRegisteredExchangeName);

            builder.RegisterType<SessionEndedSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.UserManagementConnString)
                .WithParameter("exchangeName", SessionEndedExchangeName);

            builder.RegisterType<PartnersPaymentTokensReservedSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.WalletManagementConnString)
                .WithParameter("exchangeName", PartnersPaymentTokensReservedExchangeName);
        }
    }
}
