using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    public class ProgramArguments
    {
        public string[] Arguments { get; set; }

        public bool DisplayHelp { get; set; }

        public string EdgesFilepath { get; set; }

        public string TargetNodesFilepath { get; set; }

        public string PreferredNodesFilepath { get; set; }

        public string ParametersFilepath { get; set; }

        public ProgramArguments(string[] args)
        {
            // Get the indices of the arguments.
            var edgesFilepathIndex = Array.IndexOf(args, "--edges");
            var targetNodesFilepathIndex = Array.IndexOf(args, "--targets");
            var preferredNodesFilepathIndex = Array.IndexOf(args, "--preferred");
            var parametersFilepathIndex = Array.IndexOf(args, "--parameters");
            // Assign the values to the properties.
            Arguments = args;
            DisplayHelp = args.Contains("--help");
            EdgesFilepath = edgesFilepathIndex != -1 && edgesFilepathIndex + 1 < args.Length ? args[edgesFilepathIndex + 1] : null;
            TargetNodesFilepath = targetNodesFilepathIndex != -1 && targetNodesFilepathIndex + 1 < args.Length ? args[targetNodesFilepathIndex + 1] : null;
            PreferredNodesFilepath = preferredNodesFilepathIndex != -1 && preferredNodesFilepathIndex + 1 < args.Length ? args[preferredNodesFilepathIndex + 1] : null;
            ParametersFilepath = parametersFilepathIndex != -1 && parametersFilepathIndex + 1 < args.Length ? args[parametersFilepathIndex + 1] : null;
        }
    }
}
