using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Enumerations
{
    /// <summary>
    /// Represents the possible statuses of an algorithm run.
    /// </summary>
    public enum AlgorithmStatus
    {
        /// <summary>
        /// Represents an algorithm which has been scheduled to start.
        /// </summary>
        [Display(Name = "Scheduled")]
        Scheduled,

        /// <summary>
        /// Represents an algorithm which has been chosen to start and which is currently initializing.
        /// </summary>
        [Display(Name = "Preparing to start")]
        PreparingToStart,

        /// <summary>
        /// Represents an algorithm which has been started and is still ongoing.
        /// </summary>
        [Display(Name = "Ongoing")]
        Ongoing,

        /// <summary>
        /// Represents an algorithm which has been scheduled to stop as soon as possible.
        /// </summary>
        [Display(Name = "Scheduled to stop")]
        ScheduledToStop,

        /// <summary>
        /// Represents an algorithm which has been stopped.
        /// </summary>
        [Display(Name = "Stopped")]
        Stopped,

        /// <summary>
        /// Represents an algorithm which has completed successfully.
        /// </summary>
        [Display(Name = "Completed")]
        Completed
    }
}
