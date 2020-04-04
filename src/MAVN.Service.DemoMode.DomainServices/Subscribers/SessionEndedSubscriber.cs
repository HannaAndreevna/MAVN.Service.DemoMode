using System.Threading.Tasks;
using Lykke.Common.Log;
using MAVN.Service.DemoMode.Domain.Services;
using Lykke.Service.Sessions.Contracts;

namespace MAVN.Service.DemoMode.DomainServices.Subscribers
{
    public class SessionEndedSubscriber : RabbitSubscriber<SessionEndedEvent>
    {
        private readonly IDemoModeService _demoModeService;

        public SessionEndedSubscriber(
            string connectionString,
            string exchangeName,
            IDemoModeService demoModeService,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _demoModeService = demoModeService;
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(SessionEndedEvent message)
        {
            await _demoModeService.ProcessSessionEndedAsync(message);

            return (true, string.Empty);
        }
    }
}
