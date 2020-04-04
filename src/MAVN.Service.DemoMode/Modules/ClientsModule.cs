using Autofac;
using JetBrains.Annotations;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerProfile.Client;
using MAVN.Service.DemoMode.Settings;
using Lykke.Service.PartnersIntegration.Client;
using Lykke.Service.PartnerManagement.Client;
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
