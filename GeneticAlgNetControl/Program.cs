using GeneticAlgNetControl.Helpers.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace GeneticAlgNetControl
{
    /// <summary>
    /// Represents the main class of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <param name="args">Represents the parameters for the application.</param>
        public static void Main(string[] args)
        {
            // Get the current command-line arguments configuration.
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            // Get the host to run based on the command-line arguments and build it.
            using var host = (bool.TryParse(configuration["UseCli"], out var useCli) && useCli ? CreateCliHostBuilder(args) : CreateWebHostBuilder(args)).Build();
            // Try to run it.
            try
            {
                host.Run();
            }
            catch (OperationCanceledException)
            {

            }
        }

        /// <summary>
        /// Creates a web host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new web host as defined in the "Startup" class.</returns>
        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            // Return a web host with the given parameters.
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        /// <summary>
        /// Creates a host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new host containing the given hosted service.</returns>
        public static IHostBuilder CreateCliHostBuilder(string[] args)
        {
            // Return a hosted service with the given options.
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AnalysisRunCliHostedService>();
                });
        }
    }
}
