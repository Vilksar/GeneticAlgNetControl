using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Models
{
    public class AlgorithmData
    {
        public string Id { get; set; }

        public List<(string SourceNode, string TargetNode)> Edges { get; set; }

        public List<string> Nodes { get; set; }

        public List<string> TargetNodes { get; set; }

        public List<string> PreferredNodes { get; set; }

        public string AlgorithmRunId { get; set; }

        public AlgorithmRun AlgorithmRun { get; set; }
    }
}
