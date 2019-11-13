using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents the details of a chromosome which is provided as a solution of the algorithm.
    /// </summary>
    public class ChromosomeSolution
    {
        /// <summary>
        /// Represents the fitness of the chromosome.
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Represents the number of unique control nodes of the chromosome.
        /// </summary>
        public int NumberOfUniqueControlNodes { get; set; }

        /// <summary>
        /// Represents the number of unique preferred nodes of the chromosome.
        /// </summary>
        public int NumberOfUniquePreferredControlNodes { get; set; }

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
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="chromosome"></param>
        public ChromosomeSolution(Chromosome chromosome, Dictionary<string, bool> nodeIsPreferred)
        {
            // Assign the values.
            Fitness = chromosome.GetFitness();
            UniqueControlNodes = chromosome.Genes.Values.Distinct();
            UniquePreferredControlNodes = UniqueControlNodes.Where(item => nodeIsPreferred[item]);
            NumberOfUniqueControlNodes = UniqueControlNodes.Count();
            NumberOfUniquePreferredControlNodes = UniquePreferredControlNodes.Count();
            Genes = chromosome.Genes;
        }
    }
}
