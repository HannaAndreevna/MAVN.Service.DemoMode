using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.DemoMode.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string UserManagementConnString { get; set; }

        [AmqpCheck]
        public string WalletManagementConnString { get; set; }
    }
}
