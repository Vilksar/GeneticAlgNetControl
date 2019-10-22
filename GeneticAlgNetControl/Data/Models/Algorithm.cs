﻿using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Models
{
    /// <summary>
    /// Represents the database model of an algorithm run.
    /// </summary>
    public class Algorithm
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime DateTimeStarted { get; set; }

        public DateTime? DateTimeEnded { get; set; }

        public List<DateTimePeriod> DateTimePeriods { get; set; }

        public AlgorithmStatus Status { get; set; }

        public List<Edge> Edges { get; set; }

        public List<string> Nodes { get; set; }

        public List<string> TargetNodes { get; set; }

        public List<string> PreferredNodes { get; set; }

        public int CurrentIteration { get; set; }

        public int CurrentIterationWithoutImprovement { get; set; }

        public int RandomSeed { get; set; }

        public int MaximumIterations { get; set; }

        public int MaximumIterationsWithoutImprovement { get; set; }

        public int MaximumPathLength { get; set; }

        public int PopulationSize { get; set; }

        public int RandomGenesPerChromosome { get; set; }

        public double PercentageRandom { get; set; }

        public double PercentageElite { get; set; }

        public double ProbabilityMutation { get; set; }
    }
}