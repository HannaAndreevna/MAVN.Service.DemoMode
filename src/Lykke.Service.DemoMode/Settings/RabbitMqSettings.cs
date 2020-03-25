using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.DemoMode.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string UserManagementConnString { get; set; }

        [AmqpCheck]
        public string WalletManagementConnString { get; set; }
    }
}
