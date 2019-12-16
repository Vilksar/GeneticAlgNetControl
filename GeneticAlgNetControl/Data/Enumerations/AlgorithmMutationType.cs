using System.ComponentModel.DataAnnotations;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible mutation types for an algorithm run.
    /// </summary>
    public enum AlgorithmMutationType
    {
        /// <summary>
        /// Represents the previously used mutation algorithm.
        /// </summary>
        [Display(Name = "Default")]
        Default,

        /// <summary>
        /// Represents the default, standard, mutation algorithm.
        /// </summary>
        [Display(Name = "Standard")]
        Standard,

        /// <summary>
        /// Represents the previously used mutation algorithm that mutates into preferred nodes whenever possible.
        /// </summary>
        [Display(Name = "Default with preference")]
        DefaultWithPreference,

        /// <summary>
        /// Represents the default, standard, mutation algorithm that is twice less likely to mutate preferred nodes.
        /// </summary>
        [Display(Name = "Standard with preference")]
        StandardWithPreference
    }
}
