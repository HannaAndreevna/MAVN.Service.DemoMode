using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.PartnersIntegration.Client;
using MAVN.Service.PartnerManagement.Client;

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
