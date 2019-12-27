using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static GeneticAlgNetControl.Data.Models.Analysis;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Represents the hosted service corresponding to an analysis run using the CLI.
    /// </summary>
    public class AnalysisRunCliHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AnalysisRunCliHostedService> _logger;

        /// <summary>
        /// Represents the host application lifetime.
        /// </summary>
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="logger">Represents the logger.</param>
        /// <param name="hostApplicationLifetime">Represents the application lifetime.</param>
        public AnalysisRunCliHostedService(IConfiguration configuration, ILogger<AnalysisRunCliHostedService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _configuration = configuration;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <summary>
        /// Launches an analysis run execution.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token corresponding to the task.</param>
        /// <returns>A runnable task.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Check if there is any request for displaying the help details.
            if (bool.TryParse(_configuration["Help"], out var displayHelp) && displayHelp)
            {
                // Log a message.
                _logger.LogInformation(string.Concat(
                    "\n\tWelcome to the GeneticAlgNetControl application!",
                    "\n\t",
                    "\n\t---",
                    "\n\t",
                    "\n\tThe following arguments can be provided:",
                    "\n\t--UseCli\tUse this parameter to run an analysis in the application using the CLI (and not as a local web server).",
                    "\n\t--Help\tUse this parameter to display this help message.",
                    "\n\t--Edges\tUse this parameter to specify the path to the file containing the edges of the network. Each edge should be on a new line, with its source and target nodes being separated by a tab character.",
                    "\n\t--Targets\tUse this parameter to specify the path to the file containing the target nodes of the network. Only nodes appearing in the network will be considered. Each node should be on a new line.",
                    "\n\t--Preferred\t(optional) Use this parameter to specify the path to the file containing the preferred nodes of the network. Only nodes appearing in the network will be considered. Each node should be on a new line.",
                    "\n\t--Parameters\tUse this parameter to specify the path to the file containing the parameter values for the analysis. The file should be in a JSON format, as shown in the \"DefaultParameters.json\" file, containing the default parameter values.",
                    "\n\t",
                    "\n\t---",
                    "\n\t",
                    "\n\tExamples of posible usage:",
                    "\n\t--UseCli \"True\" --Help \"True\"",
                    "\n\t--UseCli \"True\" --Edges \"Path/To/FileContainingEdges.extension\" --Targets \"Path/To/FileContainingTargetNodes.extension\" --Parameters \"Path/To/FileContainingParameters.extension\"",
                    "\n\t"));
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Get the parameters from the configuration.
            var edgesFilepath = _configuration["Edges"];
            var targetNodesFilepath = _configuration["Targets"];
            var preferredNodesFilepath = _configuration["Preferred"];
            var parametersFilepath = _configuration["Parameters"];
            // Check if we have a file containing the edges.
            if (string.IsNullOrEmpty(edgesFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the network edges has been provided.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if we have a file containing the targets.
            if (string.IsNullOrEmpty(targetNodesFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the network target nodes has been provided.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if we have a file containing the targets.
            if (string.IsNullOrEmpty(parametersFilepath))
            {
                // Log an error.
                _logger.LogError("No file containing the parameters has been provided.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Get the current directory.
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Check if the file containing the edges exists.
            if (!File.Exists(edgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{edgesFilepath}\" (containing the network edges) could not be found in the current directory \"{currentDirectory}\".");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if the file containing the target nodes exists.
            if (!File.Exists(edgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{targetNodesFilepath}\" (containing the network target nodes) could not be found in the current directory \"{currentDirectory}\".");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if the file containing the preferred nodes exists.
            if (!string.IsNullOrEmpty(preferredNodesFilepath) && !File.Exists(edgesFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{preferredNodesFilepath}\" (containing the network preferred nodes) could not be found in the current directory \"{currentDirectory}\".");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if the file containing the parameters exists.
            if (!File.Exists(parametersFilepath))
            {
                // Log an error.
                _logger.LogError($"The file \"{parametersFilepath}\" (containing the parameters) could not be found in the current directory \"{currentDirectory}\".");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Define the variables needed for the analysis.
            var nodes = new List<string>();
            var edges = new List<Edge>();
            var targetNodes = new List<string>();
            var preferredNodes = new List<string>();
            var parameters = new Parameters();
            // Try to read the edges from the file.
            try
            {
                // Read all the rows in the file and parse them into edges.
                edges = File.ReadAllLines(edgesFilepath)
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
                _logger.LogError($"An error occured while reading the file \"{edgesFilepath}\" (containing the edges).");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Try to read the target nodes from the file.
            try
            {
                // Read all the rows in the file and parse them into nodes.
                targetNodes = File.ReadAllLines(targetNodesFilepath)
                    .Where(item => !string.IsNullOrEmpty(item))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log an error.
                _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{targetNodesFilepath}\" (containing the target nodes).");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if there are any preferred nodes to read.
            if (!string.IsNullOrEmpty(preferredNodesFilepath))
            {
                // Try to read the preferred nodes from the file.
                try
                {
                    // Read all the rows in the file and parse them into nodes.
                    preferredNodes = File.ReadAllLines(preferredNodesFilepath)
                        .Where(item => !string.IsNullOrEmpty(item))
                        .Distinct()
                        .ToList();
                }
                catch (Exception ex)
                {
                    // Log an error.
                    _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{preferredNodesFilepath}\" (containing the preferred nodes).");
                    // Stop the application.
                    _hostApplicationLifetime.StopApplication();
                    // Return a successfully completed task.
                    return;
                }
            }
            // Try to read the parameters from the file.
            try
            {
                // Read the parameters file content as a JSON object.
                parameters = JsonSerializer.Deserialize<Parameters>(File.ReadAllText(parametersFilepath));
            }
            catch (Exception ex)
            {
                // Log an error.
                _logger.LogError($"The error \"{ex.Message}\" occured while reading the file \"{parametersFilepath}\" (containing the parameters).");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Check if there weren't any edges found.
            if (!edges.Any())
            {
                // Log an error.
                _logger.LogError($"No edges could be read from the file \"{edgesFilepath}\". Please check again the file and make sure that it is in the required format.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
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
                _logger.LogError($"No target nodes could be read from the file \"{targetNodesFilepath}\", or none of them could be found in the network. Please check again the file and make sure that it is in the required format.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
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
                _logger.LogError($"The parameters read from the file \"{parametersFilepath}\" are not valid. Please check again the file.");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Log a message about the loaded data.
            _logger.LogInformation($"Data loaded successfully.\n\t{edges.Count()} edges and {nodes.Count()} nodes loaded from \"{edgesFilepath}\".\n\t{targetNodes.Count()} target nodes loaded from \"{targetNodesFilepath}\". {preferredNodes.Count()} preferred nodes loaded{(string.IsNullOrEmpty(preferredNodesFilepath) ? string.Empty : $" from {preferredNodesFilepath}")}.");
            // Log a message about the parameters.
            _logger.LogInformation($"The following parameters were loaded from \"{parametersFilepath}\".\n\tRandomSeed = {parameters.RandomSeed}\n\tMaximumIterations = {parameters.MaximumIterations}\n\tMaximumIterationsWithoutImprovement = {parameters.MaximumIterationsWithoutImprovement}\n\tMaximumPathLength = {parameters.MaximumPathLength}\n\tPopulationSize = {parameters.PopulationSize}\n\tRandomGenesPerChromosome = {parameters.RandomGenesPerChromosome}\n\tPercentageRandom = {parameters.PercentageRandom}\n\tPercentageElite = {parameters.PercentageElite}\n\tProbabilityMutation = {parameters.ProbabilityMutation}\n\tCrossoverType = {parameters.CrossoverType.ToString()}\n\tMutationType = {parameters.MutationType.ToString()}");
            // Define a new analysis.
            var analysis = new Analysis(Path.GetFileNameWithoutExtension(edgesFilepath), edges, nodes, targetNodes, preferredNodes, parameters);
            // Run the analysis.
            await analysis.Run(_logger, _hostApplicationLifetime, null);
            // Get the path of the output file.
            var outputFilepath = edgesFilepath.Replace(Path.GetExtension(edgesFilepath), $"_output{(preferredNodes.Any() ? "_preferred" : string.Empty)}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json");
            // Log a message.
            _logger.LogInformation($"{DateTime.Now.ToString()}: Writing the results in JSON format to \"{outputFilepath}\".");
            // Get the text to write to the file.
            var outputText = analysis.ToJson();
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
                _logger.LogInformation($"\n{outputText}\n");
                // Stop the application.
                _hostApplicationLifetime.StopApplication();
                // Return a successfully completed task.
                return;
            }
            // Stop the application.
            _hostApplicationLifetime.StopApplication();
            // Return a successfully completed task.
            return;
        }
    }
}
