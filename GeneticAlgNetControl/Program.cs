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
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).AddCommandLine(args).Build();
            // Get the mode in which to run the application.
            var mode = configuration["Mode"];
            // Get the host to run based on the command-line arguments and build it.
            using var host = (mode == "Web" ? CreateWebHostBuilder(args) : mode == "Cli" ? CreateCliHostBuilder(args) : CreateDefaultHostBuilder(args)).Build();
            // Get the corresponding logger.
            var logger = host.Services.GetService<ILogger<Program>>();
            // Display a message.
            logger.LogInformation("The application has been started. You can press \"CTRL + C\" (\"Command + C\" on MacOS) to stop at any time.");
            // Try to run the application host.
            try
            {
                // Run the application host.
                host.Run();
            }
            catch (OperationCanceledException)
            {
                // Display a message.
                logger.LogInformation("The application has been stopped by the user.");
            }
        }

        /// <summary>
        /// Creates a web host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new web host as defined in the "Startup" class.</returns>
        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            // Return a host with the given parameters.
            return Host.CreateDefaultBuilder(args)
                .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        /// <summary>
        /// Creates a CLI host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new host containing the given hosted service.</returns>
        public static IHostBuilder CreateCliHostBuilder(string[] args)
        {
            // Return a host with the given options.
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

        /// <summary>
        /// Creates a default host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new host containing the given hosted service.</returns>
        public static IHostBuilder CreateDefaultHostBuilder(string[] args)
        {
            // Return a host with the given options.
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AnalysisRunDefaultHostedService>();
                });
        }
    }
}
