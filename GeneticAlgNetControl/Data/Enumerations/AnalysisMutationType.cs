using System.ComponentModel.DataAnnotations;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible mutation algorithms for an analysis.
    /// </summary>
    public enum AnalysisMutationType
    {
        /// <summary>
        /// Represents the default mutation algorithm.
        /// </summary>
        [Display(Name = "Weighted random ancestor")]
        WeightedRandom,

        /// <summary>
        /// Represents the default mutation algorithm that is twice less likely to mutate preferred nodes.
        /// </summary>
        [Display(Name = "Weighted random preferred ancestor")]
        WeightedRandomPreferred,

        /// <summary>
        /// Represents the default mutation algorithm that chooses a completely random mutation..
        /// </summary>
        [Display(Name = "Random ancestor")]
        Random,

        /// <summary>
        /// Represents the default mutation algorithm that mutates into preferred nodes whenever possible.
        /// </summary>
        [Display(Name = "Random preferred ancestor")]
        RandomPreferred
    }
}
