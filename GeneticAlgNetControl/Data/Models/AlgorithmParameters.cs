using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Models
{
    public class AlgorithmParameters
    {
        public string Id { get; set; }

        public int RandomSeed { get; set; }

        public int MaximumIterations { get; set; }

        public int MaximumIterationsWithoutImprovement { get; set; }

        public int MaximumPathLength { get; set; }

        public int PopulationSize { get; set; }

        public int RandomGenesPerChromosome { get; set; }

        public double PercentageRandom { get; set; }

        public double PercentageElite { get; set; }

        public double ProbabilityMutation { get; set; }

        public string AlgorithmRunId { get; set; }

        public AlgorithmRun AlgorithmRun { get; set; }
    }
}
