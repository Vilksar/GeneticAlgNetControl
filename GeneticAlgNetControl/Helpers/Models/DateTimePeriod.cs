﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents a time period in which an algorithm can run.
    /// </summary>
    public class DateTimePeriod
    {
        /// <summary>
        /// Represents the start time of the period.
        /// </summary>
        public DateTime? DateTimeStarted { get; set; }

        /// <summary>
        /// Represents the end time of the period.
        /// </summary>
        public DateTime? DateTimeEnded { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DateTimePeriod()
        {
            DateTimeStarted = null;
            DateTimeEnded = null;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="startTime">The start time of the period.</param>
        /// <param name="endTime">The end time of the period.</param>
        public DateTimePeriod(DateTime? startTime, DateTime? endTime)
        {
            DateTimeStarted = startTime ?? null;
            DateTimeEnded = endTime ?? null;
        }
    }
}
