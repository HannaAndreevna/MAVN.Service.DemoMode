using JetBrains.Annotations;
using MAVN.Service.DemoMode.Domain.Entities;

namespace MAVN.Service.DemoMode.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DemoModeSettings
    {
        public string DemoModeEmailSuffix { get; set; }

        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public PaymentRequestModelSettings PaymentRequestModel { get; set; }
    }
}
