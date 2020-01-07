using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    public class AnalysisRunDefaultHostedService : BackgroundService
    {
        /// <summary>
        /// Represents the configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Represents the logger.
        /// </summary>
        private readonly ILogger<AnalysisRunDefaultHostedService> _logger;

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
        public AnalysisRunDefaultHostedService(IConfiguration configuration, ILogger<AnalysisRunDefaultHostedService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _configuration = configuration;
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
            // Wait for a completed task, in order to not get a warning about having an async method.
            await Task.CompletedTask;
            // Log a message.
            _logger.LogInformation(string.Concat(
                "\n\tWelcome to the GeneticAlgNetControl application!",
                "\n\t",
                "\n\t---",
                "\n\t",
                "\n\tAll argument names and values are case-sensitive. The following arguments can be provided:",
                "\n\t--Mode\tUse this argument to apecify the mode in which the application will run. The possible values are \"Web\" (the application will run as a local web server), \"Cli\" (the application will run in the command-line) and \"Help\" (the application will display this help message). The default value is \"Web\".",
                "\n\tArguments for \"Web\" mode:",
                "\n\t\t--Urls\t(optional) Use this argument to specify the local address on which to run the web server. It can also be configured in the \"appsettings.json\" file. The default value is \"http://localhost:5000\".",
                "\n\tArguments for \"Cli\" mode:",
                "\n\t\t--Edges\tUse this argument to specify the path to the file containing the edges of the network. Each edge should be on a new line, with its source and target nodes being separated by a semicolon character. This argument has no default value.",
                "\n\t\t--Targets\tUse this argument to specify the path to the file containing the target nodes of the network. Only nodes appearing in the network will be considered. Each node should be on a new line. This argument has no default value.",
                "\n\t\t--Preferred\t(optional) Use this argument to specify the path to the file containing the preferred nodes of the network. Only nodes appearing in the network will be considered. Each node should be on a new line. This argument has no default value.",
                "\n\t\t--Parameters\tUse this argument to specify the path to the file containing the parameter values for the analysis. The file should be in JSON format, as shown in the \"DefaultParameters.json\" file, containing the default parameter values. This argument has no default value.",
                "\n\t\t--Output\t(optional) Use this argument to specify the path to the output file where the solutions of the analysis will be written. Writing permission is needed for the corresponding folder. If a file with the same name already exists, it will be automatically overwritten. The default value is the name of the file containing the edges, followed by the current date and time.",
                "\n\t",
                "\n\t---",
                "\n\t",
                "\n\tExamples of posible usage:",
                "\n\t--Mode \"Help\"",
                "\n\t--Mode \"Web\"",
                "\n\t--Mode \"Cli\" --Edges \"Path/To/FileContainingEdges.extension\" --Targets \"Path/To/FileContainingTargetNodes.extension\" --Parameters \"Path/To/FileContainingParameters.extension\"",
                "\n\t"));
            // Get the mode in which to run the application.
            var mode = _configuration["Mode"];
            // Check if the mode is not valid.
            if (mode != "Help")
            {
                // Log an error.
                _logger.LogError($"The provided mode \"{mode}\" for running the application is not valid.");
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
