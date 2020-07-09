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
        WeightedRandomAncestor,

        /// <summary>
        /// Represents the default mutation algorithm that is twice less likely to mutate preferred nodes.
        /// </summary>
        [Display(Name = "Weighted random preferred ancestor")]
        WeightedRandomAncestorWithPreference,

        /// <summary>
        /// Represents the default mutation algorithm that chooses a completely random mutation..
        /// </summary>
        [Display(Name = "Random ancestor")]
        RandomAncestor,

        /// <summary>
        /// Represents the default mutation algorithm that mutates into preferred nodes whenever possible.
        /// </summary>
        [Display(Name = "Random preferred ancestor")]
        RandomAncestorWithPreference
    }
}
