using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents the details of a chromosome which is provided as a solution of the analysis.
    /// </summary>
    public class Solution
    {
        /// <summary>
        /// Represents the fitness of the corresponding chromosome.
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Represents the actual maximum control path length.
        /// </summary>
        public int MaximumControlPathLength { get; set; }

        /// <summary>
        /// Represents the number of unique input nodes of the corresponding chromosome.
        /// </summary>
        public int NumberOfUniqueInputNodes { get; set; }

        /// <summary>
        /// Represents the number of unique preferred input nodes of the corresponding chromosome.
        /// </summary>
        public int NumberOfUniquePreferredInputNodes { get; set; }

        /// <summary>
        /// Represents the number of target nodes controlled by the preferred input nodes.
        /// </summary>
        public int NumberOfTargetNodesControlledByPreferredInputNodes { get; set; }

        /// <summary>
        /// Represents the unique input nodes of the corresponding chromosome.
        /// </summary>
        public IEnumerable<string> UniqueInputNodes { get; set; }

        /// <summary>
        /// Represents the unique preferred input nodes of the corresponding chromosome.
        /// </summary>
        public IEnumerable<string> UniquePreferredInputNodes { get; set; }

        /// <summary>
        /// Represents the actual gene mapping of the corresponding chromosome.
        /// </summary>
        public Dictionary<string, string> Genes { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public Solution()
        {
            // Assign the default value for each property.
            Fitness = 0.0;
            MaximumControlPathLength = -1;
            NumberOfUniqueInputNodes = 0;
            NumberOfUniquePreferredInputNodes = 0;
            NumberOfTargetNodesControlledByPreferredInputNodes = 0;
            UniqueInputNodes = Enumerable.Empty<string>();
            UniquePreferredInputNodes = Enumerable.Empty<string>();
            Genes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="chromosome">The chromosome corresponding to the solution.</param>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, its preferred status.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        public Solution(Chromosome chromosome, Dictionary<string, int> nodeIndex, Dictionary<string, bool> nodeIsPreferred, List<Matrix<double>> powersMatrixCA)
        {
            // Assign the value for each property.
            Genes = chromosome.Genes;
            Fitness = chromosome.GetFitness();
            MaximumControlPathLength = chromosome.GetMaximumPathLength(nodeIndex, powersMatrixCA);
            UniqueInputNodes = chromosome.GetUniqueInputNodes();
            NumberOfTargetNodesControlledByPreferredInputNodes = chromosome.GetNumberOfTargetsControlledByPreferredInputNodes(nodeIndex, nodeIsPreferred, powersMatrixCA);
            UniquePreferredInputNodes = UniqueInputNodes.Where(item => nodeIsPreferred[item]);
            NumberOfUniqueInputNodes = UniqueInputNodes.Count();
            NumberOfUniquePreferredInputNodes = UniquePreferredInputNodes.Count();
        }
    }
}
