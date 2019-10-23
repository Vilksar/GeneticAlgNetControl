using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible types of crossover for an algorithm run.
    /// </summary>
    public enum AlgorithmCrossoverType
    {
        /// <summary>
        /// Represents the default, standard, crossover algorithm.
        /// </summary>
        Standard,

        /// <summary>
        /// Represents the crossover algorithm which tries to choose prefer nodes first.
        /// </summary>
        WithPreference
    }
}
