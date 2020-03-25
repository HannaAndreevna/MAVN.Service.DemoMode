using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.DemoMode.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
