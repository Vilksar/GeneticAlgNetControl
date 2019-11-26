using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// Represents the previously used crossover algorithm.
        /// </summary>
        [Display(Name = "Default")]
        Default,

        /// <summary>
        /// Represents the default, standard, crossover algorithm.
        /// </summary>
        [Display(Name = "Standard")]
        Standard,

        /// <summary>
        /// Represents the previously used crossover algorithm that chooses preferred nodes whenever possible.
        /// </summary>
        [Display(Name = "Default with preference")]
        DefaultWithPreference,

        /// <summary>
        /// Represents the default, standard, crossover algorithm that is twice more likely to choose preferred nodes.
        /// </summary>
        [Display(Name = "Standard with preference")]
        StandardWithPreference
    }
}
