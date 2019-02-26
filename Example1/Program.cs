using System.IO;
using System.Threading.Tasks;
using Example1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;

namespace Example1
{
    internal static class Program
    {
        private static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(ConfigureHostConfiguration)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(ConfigureServices);

            await builder.RunConsoleAsync();
        }
        
        private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            services.AddSingleton<IHostedService, Example1Service>();
        }
        
        private static void ConfigureHostConfiguration(IConfigurationBuilder configHost)
        {
            configHost.AddEnvironmentVariables();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext hostingContext,
            IConfigurationBuilder configApp)
        {
            configApp.SetBasePath(Directory.GetCurrentDirectory());
            configApp.AddJsonFile("appsettings.json", false);
            configApp.AddEnvironmentVariables();
        }
        
        private static void ConfigureLogging(HostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            logging.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
            logging.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config", true);

            logging.AddNLog();
        }
    }
}