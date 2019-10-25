using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Interfaces;
using GeneticAlgNetControl.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Implements a Hangfire task for running an algorithm.
    /// </summary>
    public class AlgorithmRun : IAlgorithmRun
    {
        /// <summary>
        /// Represents the application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public AlgorithmRun(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Runs the algorithm with the given ID.
        /// </summary>
        /// <param name="id">The ID of the analysis to run.</param>
        /// <returns></returns>
        public async Task RunAlgorithm(string id)
        {
            // Load the algorithm with the given ID.
            var algorithm = _context.Algorithms
                .FirstOrDefault(item => item.Id == id);
            // Check if the algorithm hasn't been found.
            if (algorithm == null)
            {
                // End the function.
                return;
            }
            // Mark the algorithm for updating.
            _context.Update(algorithm);
            // Update the algorithm status and stats.
            algorithm.Status = AlgorithmStatus.Ongoing;
            algorithm.DateTimeStarted = DateTime.Now;
            // Save the changes in the database.
            await _context.SaveChangesAsync();
            // Reload it for a fresh start.
            await _context.Entry(algorithm).ReloadAsync();
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
            await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
                // Reload it for a fresh start.
                await _context.Entry(algorithm).ReloadAsync();
            }
            // Check if the algorithm doesn't exist anymore (if it has been deleted).
            if (algorithm == null)
            {
                // End the function.
                return;
            }
            // Update the solutions, end time and the status.
            algorithm.Population = population;
            algorithm.Status = algorithm.Status == AlgorithmStatus.ScheduledToStop ? AlgorithmStatus.Stopped : AlgorithmStatus.Completed;
            algorithm.DateTimeEnded = DateTime.Now;
            algorithm.DateTimePeriods = algorithm.DateTimePeriods.Append(new DateTimePeriod { DateTimeStarted = algorithm.DateTimeStarted, DateTimeEnded = algorithm.DateTimeEnded }).ToList();
            // Save the changes in the database.
            await _context.SaveChangesAsync();
        }
    }
}
