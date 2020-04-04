using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.PartnersIntegration.Client;
using Lykke.Service.PartnerManagement.Client;

namespace MAVN.Service.DemoMode.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public DemoModeSettings DemoModeService { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }

        public PartnersIntegrationServiceClientSettings PartnersIntegrationServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }

        public CustomerManagementServiceClientSettings CustomerManagementServiceClient { get; set; }
    }
}
