using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticAlgNetControl.Helpers.Models;
using GeneticAlgNetControl.Helpers.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            // Check if the program should run in the command line.
            if (bool.TryParse(config["UseCli"], out var useCli) && useCli)
            {
                // Create a new host.
                using var host = CreateHostBuilderCli(args).Build();
                // Try to start it.
                try
                {
                    host.Start();
                }
                catch (OperationCanceledException)
                {

                }
            }
            else
            {
                // Create a new host.
                using var host = CreateHostBuilder(args).Build();
                // Try to run it.
                try
                {
                    host.Run();
                }
                catch (OperationCanceledException)
                {

                }
            }
        }

        /// <summary>
        /// Creates a web host builder with the given parameters.
        /// </summary>
        /// <param name="args">Represents the parameters for the web host builder.</param>
        /// <returns>Returns a new web host as defined in the "Startup" class.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
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
        public static IHostBuilder CreateHostBuilderCli(string[] args)
        {
            // Return a hosted service with the given options.
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AlgorithmRunCliHostedService>();
                });
        }
    }
}
