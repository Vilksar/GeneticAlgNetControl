using GeneticAlgNetControl.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Models
{
    /// <summary>
    /// Represents the database model of an algorithm run.
    /// </summary>
    public class AlgorithmRun
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime DateTimeStarted { get; set; }

        public DateTime DateTimeEnded { get; set; }

        public List<(DateTime StartTime, DateTime? EndTime)> DateTimeList { get; set; }

        public AlgorithmRunStatus Status { get; set; }

        public int CurrentIteration { get; set; }

        public int CurrentIterationWithoutImprovement { get; set; }

        public string AlgorithmDataId { get; set; }

        public AlgorithmData AlgorithmData { get; set; }

        public string AlgorithmParametersId { get; set; }

        public AlgorithmParameters AlgorithmParameters { get; set; }
    }
}
