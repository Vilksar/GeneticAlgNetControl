using System.ComponentModel.DataAnnotations;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible crossover algorithms for an analysis.
    /// </summary>
    public enum AnalysisCrossoverType
    {
        /// <summary>
        /// Represents the default, standard, crossover algorithm.
        /// </summary>
        [Display(Name = "Weighted random parent")]
        WeightedRandom,

        /// <summary>
        /// Represents the default, standard, crossover algorithm that is twice more likely to choose preferred nodes.
        /// </summary>
        [Display(Name = "Weighted random preferred parent")]
        WeightedRandomWithPreference,

        /// <summary>
        /// Represents the previously used crossover algorithm.
        /// </summary>
        [Display(Name = "Dominant parent")]
        Dominant,

        /// <summary>
        /// Represents the previously used crossover algorithm that chooses preferred nodes whenever possible.
        /// </summary>
        [Display(Name = "Dominant preferred parent")]
        DominantWithPreference
    }
}
