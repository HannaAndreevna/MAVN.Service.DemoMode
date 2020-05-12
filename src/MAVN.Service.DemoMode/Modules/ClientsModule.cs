using Autofac;
using JetBrains.Annotations;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.DemoMode.Settings;
using MAVN.Service.PartnersIntegration.Client;
using MAVN.Service.PartnerManagement.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.DemoMode.Modules
{
    [UsedImplicitly]
    public class ClientsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ClientsModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterCustomerProfileClient(_appSettings.CurrentValue.CustomerProfileServiceClient);
            builder.RegisterPartnersIntegrationClient(_appSettings.CurrentValue.PartnersIntegrationServiceClient, null);
            builder.RegisterPartnerManagementClient(_appSettings.CurrentValue.PartnerManagementServiceClient);
            builder.RegisterCustomerManagementClient(_appSettings.CurrentValue.CustomerManagementServiceClient, null);
        }
    }
}
