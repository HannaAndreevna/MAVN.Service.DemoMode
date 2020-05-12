using System.Threading.Tasks;
using MAVN.Service.CustomerManagement.Contract.Events;
using MAVN.Service.PartnersPayments.Contract;
using MAVN.Service.Sessions.Contracts;

namespace MAVN.Service.DemoMode.Domain.Services
{
    public interface IDemoModeService
    {
        Task ProcessCustomerRegistrationAsync(CustomerRegistrationEvent message);

        Task ProcessSessionEndedAsync(SessionEndedEvent message);

        Task ProcessPartnersPaymentTokensReservedAsync(PartnersPaymentTokensReservedEvent message);
    }
}
