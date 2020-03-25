using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.DemoMode.Domain.Entities
{
    public class PaymentRequestModelSettings
    {
        public int PaymentRequestDelayInMilliseconds { get; set; }

        [Optional]
        public string TokensAmount { get; set; }

        [Optional]
        public decimal? FiatAmount { get; set; }

        public decimal TotalFiatAmount { get; set; }

        public string Currency { get; set; }

        [Optional]
        public int? CustomerExpirationInSeconds { get; set; }

        public string PaymentInfo { get; set; }
    }
}
