using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents the program arguments when the application is ran through the command line.
    /// </summary>
    public class ProgramArguments
    {
        /// <summary>
        /// Represents the given command-line arguments.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Represents a boolean showing if displaying the help message was requested.
        /// </summary>
        public bool DisplayHelp { get; set; }

        /// <summary>
        /// Represents the path to the file containing the network edges.
        /// </summary>
        public string EdgesFilepath { get; set; }

        /// <summary>
        /// Represents the path to the file containing the target nodes.
        /// </summary>
        public string TargetNodesFilepath { get; set; }

        /// <summary>
        /// Represents the path to the file containing the preferred nodes.
        /// </summary>
        public string PreferredNodesFilepath { get; set; }

        /// <summary>
        /// Represents the path to the file containing the parameters.
        /// </summary>
        public string ParametersFilepath { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public ProgramArguments()
        {
            // Assign the default value for each property.
            Arguments = new string[0];
            DisplayHelp = false;
            EdgesFilepath = null;
            TargetNodesFilepath = null;
            PreferredNodesFilepath = null;
            ParametersFilepath = null;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public ProgramArguments(string[] args)
        {
            // Get the indices of the arguments.
            var edgesFilepathIndex = Array.IndexOf(args, "--edges");
            var targetNodesFilepathIndex = Array.IndexOf(args, "--targets");
            var preferredNodesFilepathIndex = Array.IndexOf(args, "--preferred");
            var parametersFilepathIndex = Array.IndexOf(args, "--parameters");
            // Assign the value for each property.
            Arguments = args;
            DisplayHelp = args.Contains("--help");
            EdgesFilepath = edgesFilepathIndex != -1 && edgesFilepathIndex + 1 < args.Length ? args[edgesFilepathIndex + 1] : null;
            TargetNodesFilepath = targetNodesFilepathIndex != -1 && targetNodesFilepathIndex + 1 < args.Length ? args[targetNodesFilepathIndex + 1] : null;
            PreferredNodesFilepath = preferredNodesFilepathIndex != -1 && preferredNodesFilepathIndex + 1 < args.Length ? args[preferredNodesFilepathIndex + 1] : null;
            ParametersFilepath = parametersFilepathIndex != -1 && parametersFilepathIndex + 1 < args.Length ? args[parametersFilepathIndex + 1] : null;
        }
    }
}
