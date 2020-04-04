using System;
using JetBrains.Annotations;
using Lykke.Sdk;
using MAVN.Service.DemoMode.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MAVN.Service.DemoMode
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "DemoMode API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "DemoModeLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.DemoModeService.Db.LogsConnString;
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
