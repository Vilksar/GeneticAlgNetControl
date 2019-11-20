using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents an edge in the network.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Represents the source node of the edge.
        /// </summary>
        public string SourceNode { get; set; }

        /// <summary>
        /// Represents the target node of the edge.
        /// </summary>
        public string TargetNode { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public Edge()
        {
            // Assign the default value for each property.
            SourceNode = null;
            TargetNode = null;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="targetNode"></param>
        public Edge(string sourceNode, string targetNode)
        {
            // Assign the value for each property.
            SourceNode = sourceNode;
            TargetNode = targetNode;
        }
    }
}
