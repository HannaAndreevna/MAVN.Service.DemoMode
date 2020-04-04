using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using MAVN.Service.DemoMode.Domain.Services;
using MAVN.Service.DemoMode.DomainServices.Services;
using MAVN.Service.DemoMode.Managers;
using MAVN.Service.DemoMode.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.DemoMode.Modules
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
