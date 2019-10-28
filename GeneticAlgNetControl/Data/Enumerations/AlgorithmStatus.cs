using System;
using System.Collections.Generic;
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
        Scheduled,

        /// <summary>
        /// Represents an algorithm which has been started and is still ongoing.
        /// </summary>
        Ongoing,

        /// <summary>
        /// Represents an algorithm which has been scheduled to stop as soon as possible.
        /// </summary>
        ScheduledToStop,

        /// <summary>
        /// Represents an algorithm which has been stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Represents an algorithm which has completed successfully.
        /// </summary>
        Completed
    }
}
