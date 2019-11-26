using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Models;
using MathNet.Numerics.Random;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Represents the hosted service corresponding to a CLI algorithm run.
    /// </summary>
    public class AlgorithmRunCliHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the program arguments.
        /// </summary>
        private readonly ProgramArguments _arguments;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AlgorithmRunCliHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="arguments">Represents the program arguments.</param>
        /// <param name="logger">Represents the logger.</param>
        public AlgorithmRunCliHostedService(ProgramArguments arguments, ILogger<AlgorithmRunCliHostedService> logger)
        {
            _arguments = arguments;
            _logger = logger;
        }

        /// <summary>
        /// Launches the algorithm run execution.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token corresponding to the task.</param>
        /// <returns>A runnable task.</returns>
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Check if there is any request for displaying the help details.
            if (_arguments.DisplayHelp)
            {
                // Check if the default parameters file doesn't exist.
                if (!File.Exists("defaultParameters.json"))
                {
                    // Create it.
                    File.WriteAllText("defaultParameters.json", JsonSerializer.Serialize(new Parameters(), new JsonSerializerOptions { WriteIndented = true }));
                }
                // Log a message.
                _logger.LogInformation($"\n\tWelcome to the GeneticAlgNetControl CLI!\n\t---\n\tThe following arguments can be provided:\n\t--help\tUse this option to display this help message.\n\t--edges\tUse this option to specify the path to the file containing the network edges. Each edge should be on a new line, with the source and target nodes in an edge being separated by a tab character.\n\t--targets\tUse this option to specify the path to the file containing the target nodes. Each target node should be on a new line.\n\t--preferred\t(optional) Use this option to specify the path to the file containing the preferred nodes (if any). Each preferred node should be on a new line.\n\t--parameters\tUse this option to specify the path to the JSON file containing the parameter values for the algorithm. The parameters should be formatted as JSON, like in the \"defaultParameters.json\" file containing the default parameter values.\n\n\tExample: \"GeneticAlgNetControl cli --edges FileContainingEdges.extension --target FileContainingTargetNodes.extension\"\n");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if we have a file containing the edges.
            if (string.IsNullOrEmpty(_arguments.EdgesFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the network edges has been provided.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if we have a file containing the targets.
            if (string.IsNullOrEmpty(_arguments.TargetNodesFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the network target nodes has been provided.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if we have a file containing the targets.
            if (string.IsNullOrEmpty(_arguments.ParametersFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the parameters has been provided.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Get the current directory.
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Check if the file containing the edges exists.
            if (!File.Exists(_arguments.EdgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{_arguments.EdgesFilepath}\" (containing the network edges) could not be found in the current directory \"{currentDirectory}\".");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if the file containing the target nodes exists.
            if (!File.Exists(_arguments.EdgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{_arguments.TargetNodesFilepath}\" (containing the network target nodes) could not be found in the current directory \"{currentDirectory}\".");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if the file containing the preferred nodes exists.
            if (!string.IsNullOrEmpty(_arguments.PreferredNodesFilepath) && !File.Exists(_arguments.EdgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{_arguments.PreferredNodesFilepath}\" (containing the network preferred nodes) could not be found in the current directory \"{currentDirectory}\".");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if the file containing the parameters exists.
            if (!File.Exists(_arguments.ParametersFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{_arguments.ParametersFilepath}\" (containing the parameters) could not be found in the current directory \"{currentDirectory}\".");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Define the variables needed for the algorithm.
            var nodes = new List<string>();
            var edges = new List<Edge>();
            var targetNodes = new List<string>();
            var preferredNodes = new List<string>();
            var parameters = new Parameters();
            // Try to read the edges from the file.
            try
            {
                // Read all the rows in the file and parse them into edges.
                edges = File.ReadAllLines(_arguments.EdgesFilepath)
                    .Select(item => item.Split("\t"))
                    .Where(item => item.Length > 1)
                    .Where(item => !string.IsNullOrEmpty(item[0]) && !string.IsNullOrEmpty(item[1]))
                    .Select(item => (item[0], item[1]))
                    .Distinct()
                    .Select(item => new Edge { SourceNode = item.Item1, TargetNode = item.Item2 })
                    .ToList();
            }
            catch
            {
                // Log an error.
                _logger.LogError($"An error occured while reading the file \"{_arguments.EdgesFilepath}\" (containing the edges).");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Try to read the target nodes from the file.
            try
            {
                // Read all the rows in the file and parse them into nodes.
                targetNodes = File.ReadAllLines(_arguments.TargetNodesFilepath)
                    .Where(item => !string.IsNullOrEmpty(item))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log an error.
                _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{_arguments.TargetNodesFilepath}\" (containing the target nodes).");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if there are any preferred nodes to read.
            if (!string.IsNullOrEmpty(_arguments.PreferredNodesFilepath))
            {
                // Try to read the preferred nodes from the file.
                try
                {
                    // Read all the rows in the file and parse them into nodes.
                    preferredNodes = File.ReadAllLines(_arguments.PreferredNodesFilepath)
                        .Where(item => !string.IsNullOrEmpty(item))
                        .Distinct()
                        .ToList();
                }
                catch (Exception ex)
                {
                    // Log an error.
                    _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{_arguments.PreferredNodesFilepath}\" (containing the preferred nodes).");
                    // Return a successfully completed task.
                    return Task.CompletedTask;
                }
            }
            // Try to read the parameters from the file.
            try
            {
                // Read the parameters file content as a JSON object.
                parameters = JsonSerializer.Deserialize<Parameters>(File.ReadAllText(_arguments.ParametersFilepath));
            }
            catch (Exception ex)
            {
                // Log an error.
                _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{_arguments.ParametersFilepath}\" (containing the parameters).");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Check if there weren't any edges found.
            if (!edges.Any())
            {
                // Log an error.
                _logger.LogError($"No edges could be read from the file \"{_arguments.EdgesFilepath}\". Please check again the file and make sure that it is in the required format.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Get the actual nodes in the network.
            nodes = edges.Select(item => item.SourceNode)
                .Concat(edges.Select(item => item.TargetNode))
                .Distinct()
                .ToList();
            // Get the actual target nodes in the network.
            targetNodes = targetNodes.Intersect(nodes)
                .ToList();
            // Check if there weren't any target nodes found.
            if (!targetNodes.Any())
            {
                // Log an error.
                _logger.LogError($"No target nodes could be read from the file \"{_arguments.TargetNodesFilepath}\", or none of them could be found in the network. Please check again the file and make sure that it is in the required format.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Get the actual preferred nodes in the network.
            preferredNodes = preferredNodes.Intersect(nodes)
                .ToList();
            // Check if we should generate a new random seed.
            if (parameters.RandomSeed == -1)
            {
                // Generate a random random seed.
                parameters.RandomSeed = (new Random()).Next();
            }
            // Check if all genes should have random values.
            if (parameters.RandomGenesPerChromosome == 0 || targetNodes.Count() < parameters.RandomGenesPerChromosome)
            {
                // Update the number of random genes per chromosome.
                parameters.RandomGenesPerChromosome = targetNodes.Count();
            }
            // Check if the provided parameters are not valid.
            if (!parameters.IsValid())
            {
                // Log an error.
                _logger.LogError($"The parameters read from the file \"{_arguments.ParametersFilepath}\" are not valid. Please check again the file.");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Log a message about the loaded data.
            _logger.LogInformation($"Data loaded successfully.\n\t{edges.Count()} edges and {nodes.Count()} nodes loaded from \"{_arguments.EdgesFilepath}\".\n\t{targetNodes.Count()} target nodes loaded from \"{_arguments.TargetNodesFilepath}\". {preferredNodes.Count()} preferred nodes loaded{(string.IsNullOrEmpty(_arguments.PreferredNodesFilepath) ? string.Empty : $" from {_arguments.PreferredNodesFilepath}")}.");
            // Log a message about the parameters.
            _logger.LogInformation($"The following parameters were loaded from \"{_arguments.ParametersFilepath}\".\n\tRandomSeed = {parameters.RandomSeed}\n\tMaximumIterations = {parameters.MaximumIterations}\n\tMaximumIterationsWithoutImprovement = {parameters.MaximumIterationsWithoutImprovement}\n\tMaximumPathLength = {parameters.MaximumPathLength}\n\tPopulationSize = {parameters.PopulationSize}\n\tRandomGenesPerChromosome = {parameters.RandomGenesPerChromosome}\n\tPercentageRandom = {parameters.PercentageRandom}\n\tPercentageElite = {parameters.PercentageElite}\n\tProbabilityMutation = {parameters.ProbabilityMutation}\n\tCrossoverType = {parameters.CrossoverType.ToString()}\n\tMutationType = {parameters.MutationType.ToString()}");
            // Define a new stopwatch to measure the running time and start it.
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Algorithm started.");
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Computing the variables needed for the algorithm.");
            // Get the additional needed variables.
            var nodeIndex = Algorithm.GetNodeIndex(nodes);
            var nodeIsPreferred = Algorithm.GetNodeIsPreferred(nodes, preferredNodes);
            var matrixA = Algorithm.GetMatrixA(nodeIndex, edges);
            var matrixC = Algorithm.GetMatrixC(nodeIndex, targetNodes);
            var powersMatrixA = Algorithm.GetPowersMatrixA(matrixA, parameters.MaximumPathLength);
            var powersMatrixCA = Algorithm.GetPowersMatrixCA(matrixC, powersMatrixA);
            var targetAncestors = Algorithm.GetTargetAncestors(powersMatrixA, targetNodes, nodeIndex);
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Setting up the first population.");
            // Set up the first iteration.
            var random = new Random(parameters.RandomSeed);
            var currentIteration = 0;
            var currentIterationWithoutImprovement = 0;
            var population = new Population(nodeIndex, targetNodes, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters, random);
            var bestFitness = population.HistoricBestFitness.Max();
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}:\t{currentIteration}\t/\t{parameters.MaximumIterations}\t|\t{currentIterationWithoutImprovement}\t/\t{parameters.MaximumIterationsWithoutImprovement}\t|\t{bestFitness}\t|\t{population.HistoricAverageFitness.Last()}");
            // Move through the generations.
            while (!cancellationToken.IsCancellationRequested && currentIteration < parameters.MaximumIterations && currentIterationWithoutImprovement < parameters.MaximumIterationsWithoutImprovement)
            {
                // Move on to the next iterations.
                currentIteration += 1;
                currentIterationWithoutImprovement += 1;
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
                // Log a message.
                _logger.LogInformation($"{DateTime.Now.ToString()}:\t{currentIteration}\t/\t{parameters.MaximumIterations}\t|\t{currentIterationWithoutImprovement}\t/\t{parameters.MaximumIterationsWithoutImprovement}\t|\t{bestFitness}\t|\t{population.HistoricAverageFitness.Last()}");
            }
            // Stop the measuring watch.
            stopwatch.Stop();
            // Get the path of the output file.
            var outputFilepath = _arguments.EdgesFilepath.Replace(Path.GetExtension(_arguments.EdgesFilepath), $"_output{(preferredNodes.Any() ? "_preferred" : string.Empty)}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json");
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Writing the results in JSON format to \"{outputFilepath}\".");
            // Get the text to write to the file.
            var outputText = JsonSerializer.Serialize(new
            {
                Name = Path.GetFileNameWithoutExtension(_arguments.EdgesFilepath),
                CurrentIteration = currentIteration,
                CurrentIterationWithoutImprovement = currentIterationWithoutImprovement,
                DateTime = new
                {
                    TimeElapsed = stopwatch.Elapsed
                },
                Parameters = new
                {
                    Parameters = parameters,
                    CrossoverAlgorithm = parameters.CrossoverType.ToString(),
                    MutationAlgorithm = parameters.MutationType.ToString()
                },
                Solutions = new
                {
                    NumberOfSolutions = population.Solutions.Count(),
                    Solutions = population.Solutions
                },
                HistoricAverageFitness = population.HistoricAverageFitness,
                HistoricBestFitness = population.HistoricBestFitness
            }, new JsonSerializerOptions { WriteIndented = true });
            // Try to write to the specified file.
            try
            {
                // Write the text to the file.
                File.WriteAllText(outputFilepath, outputText);
            }
            catch (Exception ex)
            {
                // Log an error.
                _logger.LogError($"The error {ex.Message} occured while writing to the results to the file \"{outputFilepath}\". The results will be displayed in the console window instead.");
                // Log a message.
                _logger.LogError($"\n{outputText}");
                // Return a successfully completed task.
                return Task.CompletedTask;
            }
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Algorithm ended in {stopwatch.Elapsed}.");
            // Return a successfully completed task.
            return Task.CompletedTask;
        }
    }
}
