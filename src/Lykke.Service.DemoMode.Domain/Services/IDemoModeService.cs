using System.Threading.Tasks;
using Lykke.Service.CustomerManagement.Contract.Events;
using Lykke.Service.PartnersPayments.Contract;
using Lykke.Service.Sessions.Contracts;

namespace Lykke.Service.DemoMode.Domain.Services
{
    public interface IDemoModeService
    {
        Task ProcessCustomerRegistrationAsync(CustomerRegistrationEvent message);

        Task ProcessSessionEndedAsync(SessionEndedEvent message);

        Task ProcessPartnersPaymentTokensReservedAsync(PartnersPaymentTokensReservedEvent message);
    }
}
