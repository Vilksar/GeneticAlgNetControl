using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents the details of a chromosome which is provided as a solution of the analysis.
    /// </summary>
    public class ChromosomeSolution
    {
        /// <summary>
        /// Represents the fitness of the chromosome.
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Represents the actual maximum control path length.
        /// </summary>
        public int MaximumControlPathLength { get; set; }

        /// <summary>
        /// Represents the number of unique control nodes of the chromosome.
        /// </summary>
        public int NumberOfUniqueControlNodes { get; set; }

        /// <summary>
        /// Represents the number of unique preferred nodes of the chromosome.
        /// </summary>
        public int NumberOfUniquePreferredControlNodes { get; set; }

        /// <summary>
        /// Represents the number of target nodes controlled by the preferred control nodes.
        /// </summary>
        public int NumberOfTargetNodesControlledByPreferredControlNodes { get; set; }

        /// <summary>
        /// Represents the unique control nodes of the chromosome.
        /// </summary>
        public IEnumerable<string> UniqueControlNodes { get; set; }

        /// <summary>
        /// Represents the unique preferred control nodes of the chromosome.
        /// </summary>
        public IEnumerable<string> UniquePreferredControlNodes { get; set; }

        /// <summary>
        /// Represents the actual gene mapping of the chromosome.
        /// </summary>
        public Dictionary<string, string> Genes { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public ChromosomeSolution()
        {
            // Assign the default value for each property.
            Fitness = 0.0;
            MaximumControlPathLength = -1;
            NumberOfUniqueControlNodes = 0;
            NumberOfUniquePreferredControlNodes = 0;
            NumberOfTargetNodesControlledByPreferredControlNodes = 0;
            UniqueControlNodes = Enumerable.Empty<string>();
            UniquePreferredControlNodes = Enumerable.Empty<string>();
            Genes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="chromosome">The chromosome corresponding to the solution.</param>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, its preferred status.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        public ChromosomeSolution(Chromosome chromosome, Dictionary<string, int> nodeIndex, Dictionary<string, bool> nodeIsPreferred, List<Matrix<double>> powersMatrixCA)
        {
            // Assign the value for each property.
            Genes = chromosome.Genes;
            Fitness = chromosome.GetFitness();
            MaximumControlPathLength = chromosome.GetMaximumPathLength(nodeIndex, powersMatrixCA);
            UniqueControlNodes = chromosome.GetUniqueControlNodes();
            NumberOfTargetNodesControlledByPreferredControlNodes = chromosome.GetNumberOfTargetsControlledByPreferred(nodeIndex, nodeIsPreferred, powersMatrixCA);
            UniquePreferredControlNodes = UniqueControlNodes.Where(item => nodeIsPreferred[item]);
            NumberOfUniqueControlNodes = UniqueControlNodes.Count();
            NumberOfUniquePreferredControlNodes = UniquePreferredControlNodes.Count();
        }
    }
}
