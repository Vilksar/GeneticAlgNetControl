using System.ComponentModel.DataAnnotations;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible statuses of an analysis.
    /// </summary>
    public enum AnalysisStatus
    {
        /// <summary>
        /// Represents an analysis which has been scheduled to start.
        /// </summary>
        [Display(Name = "Scheduled")]
        Scheduled,

        /// <summary>
        /// Represents an analysis which has been chosen to start and which is currently initializing.
        /// </summary>
        [Display(Name = "Initializing")]
        Initializing,

        /// <summary>
        /// Represents an analysis which has been started and is still ongoing.
        /// </summary>
        [Display(Name = "Ongoing")]
        Ongoing,

        /// <summary>
        /// Represents an analysis which has been scheduled to stop as soon as possible.
        /// </summary>
        [Display(Name = "Stopping")]
        Stopping,

        /// <summary>
        /// Represents an analysis which has been stopped.
        /// </summary>
        [Display(Name = "Stopped")]
        Stopped,

        /// <summary>
        /// Represents an analysis which has completed successfully.
        /// </summary>
        [Display(Name = "Completed")]
        Completed
    }
}
