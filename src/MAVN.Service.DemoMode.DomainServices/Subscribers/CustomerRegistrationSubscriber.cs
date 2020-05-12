using System.Threading.Tasks;
using Lykke.Common.Log;
using MAVN.Service.CustomerManagement.Contract.Events;
using MAVN.Service.DemoMode.Domain.Services;

namespace MAVN.Service.DemoMode.DomainServices.Subscribers
{
    public class CustomerRegistrationSubscriber : RabbitSubscriber<CustomerRegistrationEvent>
    {
        private readonly IDemoModeService _demoModeService;

        public CustomerRegistrationSubscriber(
            string connectionString,
            string exchangeName,
            IDemoModeService demoModeService,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _demoModeService = demoModeService;
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(
            CustomerRegistrationEvent message)
        {
            await _demoModeService.ProcessCustomerRegistrationAsync(message);

            return (true, string.Empty);
        }
    }
}
