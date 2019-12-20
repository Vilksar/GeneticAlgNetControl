using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Represents the hosted service corresponding to an analysis run.
    /// </summary>
    public class AnalysisRunHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the service scope factory.
        /// </summary>
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AnalysisRunHostedService> _logger;

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
        public AnalysisRunHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<AnalysisRunHostedService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <summary>
        /// Launches the algorithm run execution.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token corresponding to the task.</param>
        /// <returns>A runnable task.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Use the current scope.
            using var scope = _serviceScopeFactory.CreateScope();
            // Get the application context.
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Go over each algorithm in the database that is ongoing at start.
            foreach (var algorithm in context.Analyses.Where(item => item.Status == AnalysisStatus.Ongoing))
            {
                // Update its status.
                algorithm.Status = AnalysisStatus.Scheduled;
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
                // Mark the analysis for updating.
                context.Update(analysis);
                // Update the analysis status and stats.
                analysis.Status = AnalysisStatus.Initializing;
                analysis.DateTimeStarted = DateTime.Now;
                analysis.DateTimePeriods = JsonSerializer.Serialize(JsonSerializer.Deserialize<List<DateTimePeriod>>(analysis.DateTimePeriods).Append(new DateTimePeriod(analysis.DateTimeStarted, null)));
                // Save the changes in the database.
                await context.SaveChangesAsync();
                // Reload it for a fresh start.
                await context.Entry(analysis).ReloadAsync();
                // Get the edges, nodes, target nodes, preferred nodes and parameters.
                var nodes = JsonSerializer.Deserialize<List<string>>(analysis.Nodes);
                var edges = JsonSerializer.Deserialize<List<Edge>>(analysis.Edges);
                var targetNodes = JsonSerializer.Deserialize<List<string>>(analysis.TargetNodes);
                var preferredNodes = JsonSerializer.Deserialize<List<string>>(analysis.PreferredNodes);
                var parameters = JsonSerializer.Deserialize<Parameters>(analysis.Parameters);
                // Get the additional needed variables.
                var nodeIndex = Analysis.GetNodeIndex(nodes);
                var nodeIsPreferred = Analysis.GetNodeIsPreferred(nodes, preferredNodes);
                var matrixA = Analysis.GetMatrixA(nodeIndex, edges);
                var matrixC = Analysis.GetMatrixC(nodeIndex, targetNodes);
                var powersMatrixA = Analysis.GetPowersMatrixA(matrixA, parameters.MaximumPathLength);
                var powersMatrixCA = Analysis.GetPowersMatrixCA(matrixC, powersMatrixA);
                var targetAncestors = Analysis.GetTargetAncestors(powersMatrixA, targetNodes, nodeIndex);
                // Update the analysis status.
                analysis.Status = AnalysisStatus.Ongoing;
                // Save the changes in the database.
                await context.SaveChangesAsync();
                // Set up the current iteration.
                var random = new Random(parameters.RandomSeed);
                var currentIteration = analysis.CurrentIteration;
                var currentIterationWithoutImprovement = analysis.CurrentIterationWithoutImprovement;
                var population = JsonSerializer.Deserialize<Population>(analysis.Population);
                // Check if the current population is empty.
                if (!population.Chromosomes.Any())
                {
                    // Initialize a new population.
                    population = new Population(nodeIndex, targetNodes, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters, random);
                }
                // Get the best fitness so far.
                var bestFitness = population.HistoricBestFitness.Max();
                // Move through the generations.
                while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested && analysis != null && analysis.Status == AnalysisStatus.Ongoing && currentIteration < parameters.MaximumIterations && currentIterationWithoutImprovement < parameters.MaximumIterationsWithoutImprovement)
                {
                    // Move on to the next iterations.
                    currentIteration += 1;
                    currentIterationWithoutImprovement += 1;
                    // Update the iteration count.
                    analysis.CurrentIteration = currentIteration;
                    analysis.CurrentIterationWithoutImprovement = currentIterationWithoutImprovement;
                    // Save the changes in the database.
                    await context.SaveChangesAsync();
                    // Move on to the next population.
                    population = new Population(population, nodeIndex, targetNodes, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters, random);
                    // Get the best fitness of the current population.
                    var fitness = population.HistoricBestFitness.Last();
                    // Check if the current solution is better than the previous solution.
                    if (bestFitness < fitness)
                    {
                        // Update the fitness.
                        bestFitness = fitness;
                        currentIterationWithoutImprovement = 0;
                    }
                    // Reload it for a fresh start.
                    await context.Entry(analysis).ReloadAsync();
                }
                // Check if the analysis doesn't exist anymore (if it has been deleted).
                if (analysis == null)
                {
                    // End the function.
                    continue;
                }
                // Update the solutions, end time and the status.
                analysis.Population = JsonSerializer.Serialize(population);
                analysis.Status = analysis.Status == AnalysisStatus.Stopping ? AnalysisStatus.Stopped : AnalysisStatus.Completed;
                analysis.DateTimeEnded = DateTime.Now;
                analysis.DateTimePeriods = JsonSerializer.Serialize(JsonSerializer.Deserialize<List<DateTimePeriod>>(analysis.DateTimePeriods).SkipLast(1).Append(new DateTimePeriod(analysis.DateTimeStarted, analysis.DateTimeEnded)));
                // Save the changes in the database.
                await context.SaveChangesAsync();
            }
            // Stop the application.
            _hostApplicationLifetime.StopApplication();
        }
    }
}
