using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Represents the hosted service corresponding to an algorithm run.
    /// </summary>
    public class AlgorithmRunHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the service scope factory.
        /// </summary>
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AlgorithmRunHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="serviceScopeFactory">Represents the service scope factory.</param>
        /// <param name="logger">Represents the logger.</param>
        public AlgorithmRunHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<AlgorithmRunHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Launches the algorithm run execution.
        /// </summary>
        /// <param name="stopToken">The cancellation token corresponding to the task.</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            // Get the application context.
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Go over each algorithm in the database that are ongoing at start.
            foreach (var algorithm in context.Algorithms.Where(item => item.Status == AlgorithmStatus.Ongoing))
            {
                // Update its status.
                algorithm.Status = AlgorithmStatus.Scheduled;
            }
            // Save the changes to the database.
            await context.SaveChangesAsync();
            // Repeat the task.
            while (!stopToken.IsCancellationRequested)
            {
                // Get the first scheduled algorithms in the database.
                var algorithm = context.Algorithms.FirstOrDefault(item => item.Status == AlgorithmStatus.Scheduled);
                // Check if there wasn't any algorithm found.
                if (algorithm == null)
                {
                    // Wait for 30 seconds.
                    await Task.Delay(30000);
                    // Continue.
                    continue;
                }
                // Mark the algorithm for updating.
                context.Update(algorithm);
                // Update the algorithm status and stats.
                algorithm.Status = AlgorithmStatus.Ongoing;
                algorithm.DateTimeStarted = DateTime.Now;
                algorithm.DateTimePeriods = algorithm.DateTimePeriods.Append(new DateTimePeriod { DateTimeStarted = algorithm.DateTimeStarted, DateTimeEnded = null }).ToList();
                // Save the changes in the database.
                await context.SaveChangesAsync();
                // Reload it for a fresh start.
                await context.Entry(algorithm).ReloadAsync();
                // Get the edges, nodes, target nodes and preferred nodes.
                var nodes = algorithm.Nodes;
                var edges = algorithm.Edges;
                var targetNodes = algorithm.TargetNodes;
                var preferredNodes = algorithm.PreferredNodes;
                // Get the parameters.
                var randomSeed = algorithm.RandomSeed;
                var maximumIterations = algorithm.MaximumIterations;
                var maximumIterationsWithoutImprovement = algorithm.MaximumIterationsWithoutImprovement;
                var maximumPathLength = algorithm.MaximumPathLength;
                var populationSize = algorithm.PopulationSize;
                var randomGenesPerChromosome = algorithm.RandomGenesPerChromosome;
                var percentageElite = algorithm.PercentageElite;
                var percentageRandom = algorithm.PercentageRandom;
                var probabilityMutation = algorithm.ProbabilityMutation;
                var crossoverType = algorithm.CrossoverType;
                var mutationType = algorithm.MutationType;
                // Get the additional needed variables.
                var nodeIndices = Functions.Functions.GetNodeIndices(nodes);
                var nodePreferred = Functions.Functions.GetNodePreferred(nodes, preferredNodes);
                var matrixA = Functions.Functions.GetMatrixA(nodeIndices, edges);
                var matrixC = Functions.Functions.GetMatrixC(nodeIndices, targetNodes);
                var adjacencyPowerList = Functions.Functions.GetAdjacencyMatrixPowers(matrixA, maximumPathLength);
                var pathDictionary = Functions.Functions.GetPathList(adjacencyPowerList, targetNodes, nodeIndices);
                var matrixPowerList = Functions.Functions.GetMatrixPowers(matrixC, adjacencyPowerList);
                // Set up the current iteration.
                var random = new Random(randomSeed);
                var currentIteration = algorithm.CurrentIteration;
                var currentIterationWithoutImprovement = algorithm.CurrentIterationWithoutImprovement;
                var population = !algorithm.Population.Chromosomes.Any() ? new Population(populationSize, nodeIndices, targetNodes, pathDictionary, matrixPowerList, random, randomGenesPerChromosome) : algorithm.Population;
                var bestFitness = population.HistoricBestFitness.Max();
                // Save the changes in the database.
                await context.SaveChangesAsync();
                // Move through the generations.
                while (algorithm != null && algorithm.Status == AlgorithmStatus.Ongoing && currentIteration < maximumIterations && currentIterationWithoutImprovement < maximumIterationsWithoutImprovement)
                {
                    // Move on to the next iterations.
                    currentIteration += 1;
                    currentIterationWithoutImprovement += 1;
                    // Move on to the next population.
                    population = new Population(population, nodeIndices, targetNodes, pathDictionary, matrixPowerList, nodePreferred, crossoverType, mutationType, randomGenesPerChromosome, percentageElite, percentageRandom, probabilityMutation, random);
                    // Get the best fitness of the current population.
                    var fitness = population.HistoricBestFitness.Last();
                    // Check if the current solution is better than the previous solution.
                    if (bestFitness < fitness)
                    {
                        // Update the fitness.
                        bestFitness = fitness;
                        currentIterationWithoutImprovement = 0;
                    }
                    // Update the iteration count.
                    algorithm.CurrentIteration = currentIteration;
                    algorithm.CurrentIterationWithoutImprovement = currentIterationWithoutImprovement;
                    // Save the changes in the database.
                    await context.SaveChangesAsync();
                    // Reload it for a fresh start.
                    await context.Entry(algorithm).ReloadAsync();
                }
                // Check if the algorithm doesn't exist anymore (if it has been deleted).
                if (algorithm == null)
                {
                    // End the function.
                    continue;
                }
                // Update the solutions, end time and the status.
                algorithm.Population = population;
                algorithm.Status = algorithm.Status == AlgorithmStatus.ScheduledToStop ? AlgorithmStatus.Stopped : AlgorithmStatus.Completed;
                algorithm.DateTimeEnded = DateTime.Now;
                algorithm.DateTimePeriods = algorithm.DateTimePeriods.SkipLast(1).Append(new DateTimePeriod { DateTimeStarted = algorithm.DateTimeStarted, DateTimeEnded = algorithm.DateTimeEnded }).ToList();
                // Save the changes in the database.
                await context.SaveChangesAsync();
            }
        }
    }
}
