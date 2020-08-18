using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bishop.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlackNotifierWS.Service;
using ThermalNotifierWS.Service;
using ThermometerWS.Service;

namespace Bishop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<ILogger>((serviceProvider) => serviceProvider.GetService<ILogger<Worker>>());
                    services.AddHttpClient<ISlackNotifierService, SlackNotifierService>();
                    services.AddSingleton<IThermometerService, ThermometerService>();
                    services.AddSingleton<IThermalNotifierService, ThermalNotifierService>();
                    services.AddSingleton<IBishopService, BishopService>();
                    services.AddHostedService<Worker>();
                });
    }
}
