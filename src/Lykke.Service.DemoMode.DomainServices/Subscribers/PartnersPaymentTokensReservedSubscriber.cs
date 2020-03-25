using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.DemoMode.Domain.Services;
using Lykke.Service.PartnersPayments.Contract;

namespace Lykke.Service.DemoMode.DomainServices.Subscribers
{
    public class PartnersPaymentTokensReservedSubscriber : RabbitSubscriber<PartnersPaymentTokensReservedEvent>
    {
        private readonly IDemoModeService _demoModeService;

        public PartnersPaymentTokensReservedSubscriber(
            string connectionString,
            string exchangeName,
            IDemoModeService demoModeService,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _demoModeService = demoModeService;
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(PartnersPaymentTokensReservedEvent message)
        {
            await _demoModeService.ProcessPartnersPaymentTokensReservedAsync(message);

            return (true, string.Empty);
        }
    }
}
