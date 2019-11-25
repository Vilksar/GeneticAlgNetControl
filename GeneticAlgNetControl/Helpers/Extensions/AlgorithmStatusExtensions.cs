using GeneticAlgNetControl.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Extensions
{
    /// <summary>
    /// Provides extensions for the algorithm status.
    /// </summary>
    public static class AlgorithmStatusExtensions
    {
        /// <summary>
        /// Returns the actual string of the current algorithm status, in title-case ("TitleCase").
        /// </summary>
        /// <param name="algorithmStatus">The algorithm status to return as string.</param>
        /// <returns>The current algorithm status, as a title-case string.</returns>
        public static string ToActualString(this AlgorithmStatus algorithmStatus)
        {
            // Define the variable to return.
            var actualString = string.Empty;
            // Check which algorithm status we have.
            switch (algorithmStatus)
            {
                case AlgorithmStatus.Scheduled:
                    actualString = "Scheduled";
                    break;
                case AlgorithmStatus.PreparingToStart:
                    actualString = "PreparingToStart";
                    break;
                case AlgorithmStatus.Ongoing:
                    actualString = "Ongoing";
                    break;
                case AlgorithmStatus.ScheduledToStop:
                    actualString = "ScheduledToStop";
                    break;
                case AlgorithmStatus.Stopped:
                    actualString = "Stopped";
                    break;
                case AlgorithmStatus.Completed:
                    actualString = "Completed";
                    break;
                default:
                    break;
            }
            // Return the actual string.
            return actualString;
        }
    }
}
