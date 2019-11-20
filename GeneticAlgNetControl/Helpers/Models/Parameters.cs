using GeneticAlgNetControl.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents the parameters for the algorithm.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Represents the random seed to be used in the algorithm.
        /// </summary>
        public int RandomSeed { get; set; }

        /// <summary>
        /// Represents the maximum number of iteration for which the algorithm to run.
        /// </summary>
        public int MaximumIterations { get; set; }

        /// <summary>
        /// Represents the maximum number of iterations without improvement for which the algorithm to run.
        /// </summary>
        public int MaximumIterationsWithoutImprovement { get; set; }

        /// <summary>
        /// Represents the maximum path length to be used in the algorithm.
        /// </summary>
        public int MaximumPathLength { get; set; }

        /// <summary>
        /// Represents the number of chromosomes in each population.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Represents the maximum number of genes whose value can be simultaneously randomly generated.
        /// </summary>
        public int RandomGenesPerChromosome { get; set; }

        /// <summary>
        /// Represents the percentage of a population which is composed of randomly generated chromosomes.
        /// </summary>
        public double PercentageRandom { get; set; }

        /// <summary>
        /// Represents the percentage of a population which is composed of the elite chromosomes of the previous population.
        /// </summary>
        public double PercentageElite { get; set; }

        /// <summary>
        /// Represents the probability of mutation for each gene of a chromosome..
        /// </summary>
        public double ProbabilityMutation { get; set; }

        /// <summary>
        /// Represents the crossover algorithm to be used.
        /// </summary>
        public AlgorithmCrossoverType CrossoverType { get; set; }

        /// <summary>
        /// Represents the mutation algorithm to be used.
        /// </summary>
        public AlgorithmMutationType MutationType { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public Parameters()
        {
            // Assign the default value for each parameter.
            RandomSeed = DefaultValues.RandomSeed;
            MaximumIterations = DefaultValues.MaximumIterations;
            MaximumIterationsWithoutImprovement = DefaultValues.MaximumIterationsWithoutImprovement;
            MaximumPathLength = DefaultValues.MaximumPathLength;
            PopulationSize = DefaultValues.PopulationSize;
            RandomGenesPerChromosome = DefaultValues.RandomGenesPerChromosome;
            PercentageRandom = DefaultValues.PercentageRandom;
            PercentageElite = DefaultValues.PercentageElite;
            ProbabilityMutation = DefaultValues.ProbabilityMutation;
            CrossoverType = DefaultValues.CrossoverType;
            MutationType = DefaultValues.MutationType;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="randomSeed">The random seed.</param>
        /// <param name="maximumIterations">The maximum number of iterations.</param>
        /// <param name="maximumIterationsWithoutImprovement">The maximum number of iterations without improvement.</param>
        /// <param name="maximumPathLength">The maximum path length.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="randomGenesPerChromosome">The maximum number of genes whose value can be simultaneously randomly generated.</param>
        /// <param name="percentageRandom">The percentage of a population which is composed of randomly generated chromosomes.</param>
        /// <param name="percentageElite">The percentage of a population which is composed of the elite chromosomes of the previous population.</param>
        /// <param name="probabilityMutation">The probability of mutation for each gene of a chromosome.</param>
        /// <param name="crossoverType">The crossover algorithm to be used.</param>
        /// <param name="mutationType">The mutation algorithm to be used.</param>
        public Parameters(int randomSeed, int maximumIterations, int maximumIterationsWithoutImprovement, int maximumPathLength, int populationSize, int randomGenesPerChromosome, double percentageRandom, double percentageElite, double probabilityMutation, AlgorithmCrossoverType crossoverType, AlgorithmMutationType mutationType)
        {
            // Assign the value for each parameter.
            RandomSeed = randomSeed;
            MaximumIterations = maximumIterations;
            MaximumIterationsWithoutImprovement = maximumIterationsWithoutImprovement;
            MaximumPathLength = maximumPathLength;
            PopulationSize = populationSize;
            RandomGenesPerChromosome = randomGenesPerChromosome;
            PercentageRandom = percentageRandom;
            PercentageElite = percentageElite;
            ProbabilityMutation = probabilityMutation;
            CrossoverType = crossoverType;
            MutationType = mutationType;
        }

        /// <summary>
        /// Check if the parameters in the current instance are valid.
        /// </summary>
        /// <returns>True if all of the parameters are valid, false otherwise.</returns>
        public bool IsValid()
        {
            // Check if the given parameters are valid.
            return 0 <= RandomSeed &&
                1 <= MaximumIterations &&
                1 <= MaximumIterationsWithoutImprovement &&
                1 <= MaximumPathLength &&
                2 <= PopulationSize &&
                1 <= RandomGenesPerChromosome &&
                0.0 <= PercentageRandom && PercentageRandom <= 1.0 &&
                0.0 <= PercentageElite && PercentageElite <= 1.0 &&
                0.0 <= ProbabilityMutation && ProbabilityMutation <= 1.0 &&
                Enum.IsDefined(typeof(AlgorithmCrossoverType), CrossoverType) &&
                Enum.IsDefined(typeof(AlgorithmMutationType), MutationType);
        }

        /// <summary>
        /// Represents the default values for the parameters.
        /// </summary>
        public static class DefaultValues
        {
            /// <summary>
            /// Represents the default value for the random seed to be used in the algorithm.
            /// </summary>
            public static int RandomSeed { get; } = (new Random()).Next();

            /// <summary>
            /// Represents the default value for the maximum number of iteration for which the algorithm to run.
            /// </summary>
            public static int MaximumIterations { get; } = 10000;

            /// <summary>
            /// Represents the default value for the maximum number of iterations without improvement for which the algorithm to run.
            /// </summary>
            public static int MaximumIterationsWithoutImprovement { get; } = 1000;

            /// <summary>
            /// Represents the default value for the maximum path length to be used in the algorithm.
            /// </summary>
            public static int MaximumPathLength { get; } = 5;

            /// <summary>
            /// Represents the default value for the number of chromosomes in each population.
            /// </summary>
            public static int PopulationSize { get; } = 80;

            /// <summary>
            /// Represents the default value for the maximum number of genes whose value can be simultaneously randomly generated.
            /// </summary>
            public static int RandomGenesPerChromosome { get; } = 25;

            /// <summary>
            /// Represents the default value for the percentage of a population which is composed of randomly generated chromosomes.
            /// </summary>
            public static double PercentageRandom { get; } = 0.25;

            /// <summary>
            /// Represents the default value for the percentage of a population which is composed of the elite chromosomes of the previous population.
            /// </summary>
            public static double PercentageElite { get; } = 0.25;

            /// <summary>
            /// Represents the default value for the probability of mutation for each chromosome.
            /// </summary>
            public static double ProbabilityMutation { get; } = 0.01;

            /// <summary>
            /// Represents the default value for the crossover algorithm to be used.
            /// </summary>
            public static AlgorithmCrossoverType CrossoverType { get; } = AlgorithmCrossoverType.Standard;

            /// <summary>
            /// Represents the default value for the mutation algorithm to be used.
            /// </summary>
            public static AlgorithmMutationType MutationType { get; } = AlgorithmMutationType.Standard;
        }
    }
}
