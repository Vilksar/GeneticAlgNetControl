using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible mutation types for an algorithm run.
    /// </summary>
    public enum AlgorithmMutationType
    {
        /// <summary>
        /// Represents the default, standard, mutation algorithm.
        /// </summary>
        [Display(Name = "Standard")]
        Standard,

        /// <summary>
        /// Represents the default, standard, mutation algorithm that is less likely to mutate preferred nodes.
        /// </summary>
        [Display(Name = "Standard with preference")]
        StandardWithPreference
    }
}
