using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.DemoMode.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
