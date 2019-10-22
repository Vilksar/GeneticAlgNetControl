using GeneticAlgNetControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.ViewModels
{
    public class AlgorithmOverviewViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public double ProgressIterations { get; set; }

        public double ProgressIterationsWithoutImprovement { get; set; }

        public AlgorithmOverviewViewModel(Algorithm algorithm)
        {
            Id = algorithm.Id;
            Name = algorithm.Name;
            TimeSpan = algorithm.DateTimePeriods.Select(item => (item.DateTimeEnded ?? DateTime.Now) - item.DateTimeStarted).Aggregate(TimeSpan.Zero, (sum, item) => sum + item);
            Status = algorithm.Status.ToString();
            ProgressIterations = (double)algorithm.CurrentIteration / algorithm.MaximumIterations;
            ProgressIterationsWithoutImprovement = (double)algorithm.CurrentIterationWithoutImprovement / algorithm.MaximumIterationsWithoutImprovement;
        }
    }
}
