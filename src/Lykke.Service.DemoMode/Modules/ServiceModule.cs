using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.DemoMode.Domain.Services;
using Lykke.Service.DemoMode.DomainServices.Services;
using Lykke.Service.DemoMode.Managers;
using Lykke.Service.DemoMode.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.DemoMode.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DemoModeService>()
                .As<IDemoModeService>()
                .WithParameter("paymentRequestSettings", _appSettings.CurrentValue.DemoModeService.PaymentRequestModel)
                .WithParameter("demoModeEmailSuffix", _appSettings.CurrentValue.DemoModeService.DemoModeEmailSuffix)
                .InstancePerDependency();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance()
                .AutoActivate();
        }
    }
}
