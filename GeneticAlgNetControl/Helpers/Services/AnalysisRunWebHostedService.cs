﻿using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Represents the hosted service corresponding to an analysis run.
    /// </summary>
    public class AnalysisRunWebHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AnalysisRunWebHostedService> _logger;

        /// <summary>
        /// Represents the host application lifetime.
        /// </summary>
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="serviceScopeFactory">Represents the service scope factory.</param>
        /// <param name="logger">Represents the logger.</param>
        /// <param name="hostApplicationLifetime">Represents the application lifetime.</param>
        public AnalysisRunWebHostedService(IServiceProvider serviceProvider, ILogger<AnalysisRunWebHostedService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <summary>
        /// Executes the background service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token corresponding to the task.</param>
        /// <returns>A runnable task.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Wait for 10 seconds for the web server to start.
            await Task.Delay(10000);
            // Create a new scope.
            using var scope = _serviceProvider.CreateScope();
            // Use a new context instance.
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Go over each analysis in the database that is ongoing at start.
            foreach (var analysis in context.Analyses.Where(item => item.Status == AnalysisStatus.Initializing || item.Status == AnalysisStatus.Ongoing || item.Status == AnalysisStatus.Stopping))
            {
                // Update its status.
                analysis.Status = AnalysisStatus.Scheduled;
            }
            // Save the changes to the database.
            await context.SaveChangesAsync();
            // Repeat the task.
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                // Get the first scheduled analysis in the database.
                var analysis = context.Analyses.FirstOrDefault(item => item.Status == AnalysisStatus.Scheduled);
                // Check if there wasn't any analysis found.
                if (analysis == null)
                {
                    // Wait for 30 seconds.
                    await Task.Delay(30000);
                    // Continue.
                    continue;
                }
                // Run the analysis.
                await analysis.Run(_logger, _hostApplicationLifetime, context);
            }
            // Stop the application.
            _hostApplicationLifetime.StopApplication();
        }
    }
}
