using GeneticAlgNetControl.Data.Enumerations;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents a population of chromosomes used by the algorithm.
    /// </summary>
    public class Population
    {
        /// <summary>
        /// Represents the fitness of the chromosomes in the population (with the same length as the population).
        /// </summary>
        private List<double> _fitnessList = null;

        /// <summary>
        /// Represents the combined fitness of the chromosomes in the population (one element bigger than the population, an extra 0.0 as first element).
        /// </summary>
        private List<double> _fitnessCombinedList = null;

        /// <summary>
        /// Represents the chromosomes in the population.
        /// </summary>
        public List<Chromosome> Chromosomes { get; set; }

        /// <summary>
        /// Represents the list of best fitnesses over consequent populations.
        /// </summary>
        public List<double> HistoricBestFitness { get; set; }

        /// <summary>
        /// Represents the list of average fitnesses over consequent populations.
        /// </summary>
        public List<double> HistoricAverageFitness { get; set; }

        /// <summary>
        /// Constructor for an empty population.
        /// </summary>
        public Population()
        {
            Chromosomes = new List<Chromosome>();
            HistoricBestFitness = new List<double>();
            HistoricAverageFitness = new List<double>();
        }

        /// <summary>
        /// Constructor for the initial population.
        /// </summary>
        /// <param name="populationSize">The size of the population.</param>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <param name="pathList">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="matrixPowerList">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="random">The random seed.</param>
        public Population(Dictionary<string, int> nodeIndices, List<string> targetNodes, Dictionary<string, List<string>> pathList, List<Matrix<double>> matrixPowerList, Parameters parameters, Random random)
        {
            // Initialize the list of chromosomes.
            Chromosomes = new List<Chromosome>();
            // Get the number of elements in each group and the minimum number of elements per group.
            var numberOfGroups = (int)Math.Ceiling((double)targetNodes.Count() / (double)parameters.RandomGenesPerChromosome);
            var chromosomesPerGroup = (int)Math.Floor((double)populationSize / (double)numberOfGroups);
            var genesPerGroup = (int)Math.Floor((double)targetNodes.Count() / (double)numberOfGroups);
            var numberOfChromosomeGroupsExtra = populationSize - chromosomesPerGroup * numberOfGroups;
            var numberOfGeneGroupsExtra = targetNodes.Count() - genesPerGroup * numberOfGroups;
            // Get the actual number of chromosomes in each group.
            var chromosomeGroups = new List<int> { 0 }
                .Concat(Enumerable.Range(0, numberOfChromosomeGroupsExtra).Select(item => chromosomesPerGroup + 1))
                .Concat(Enumerable.Range(numberOfChromosomeGroupsExtra, numberOfGroups).Select(item => chromosomesPerGroup))
                .ToList();
            // Get the actual numer of genes in each group.
            var sum = 0;
            var geneGroups = new List<int> { 0 }
                .Concat(Enumerable.Range(0, numberOfGeneGroupsExtra).Select(item => genesPerGroup + 1))
                .Concat(Enumerable.Range(numberOfGeneGroupsExtra, numberOfGroups).Select(item => genesPerGroup))
                .Select(item => sum += item)
                .ToList();
            // Repeat for each group.
            for (int index1 = 1; index1 < numberOfGroups + 1; index1++)
            {
                // Get the lower and upper limits.
                var lowerLimit = geneGroups[index1 - 1];
                var upperLimit = geneGroups[index1];
                // Repeat for the number of elements in the group.
                for (int index2 = 0; index2 < chromosomeGroups[index1]; index2++)
                {
                    // Add a new, initialized, chromosome.
                    Chromosomes.Add(new Chromosome(targetNodes).Initialize(nodeIndices, pathList, matrixPowerList, lowerLimit, upperLimit, random));
                }
            }
            // Define the historic best and average fitness.
            HistoricBestFitness = new List<double> { GetFitnessList().Max() };
            HistoricAverageFitness = new List<double> { GetFitnessList().Average() };
        }

        /// <summary>
        /// Constructor for a subsequent population.
        /// </summary>
        /// <param name="populationSize"></param>
        public Population(Population previousPopulation, Dictionary<string, int> nodeIndices, List<string> targetNodes, Dictionary<string, List<string>> pathList, List<Matrix<double>> matrixPowerList, Dictionary<string, bool> nodePreferred, Parameters parameters, Random random)
        {
            // Initialize the list of chromosomes.
            Chromosomes = new List<Chromosome>();
            // Get the fitness list of the population.
            var fitness = previousPopulation.GetCombinedFitnessList();
            // Add the specified number of elite chromosomes from the previous population.
            Chromosomes.AddRange(previousPopulation.Chromosomes.OrderByDescending(item => item.GetFitness()).Take((int)Math.Floor(parameters.PercentageElite * previousPopulation.Chromosomes.Count())));
            // Add the specified number of random chromosomes.
            for (int index = 0; index < (int)Math.Floor(parameters.PercentageRandom * previousPopulation.Chromosomes.Count()); index++)
            {
                // Get the lower and upper limits.
                var lowerLimit = random.Next(Math.Max(Math.Min(pathList.Count(), pathList.Count() - parameters.RandomGenesPerChromosome), 0));
                var upperLimit = Math.Min(lowerLimit + parameters.RandomGenesPerChromosome, pathList.Count());
                // Add a new, initialized, chromosome.
                Chromosomes.Add(new Chromosome(targetNodes).Initialize(nodeIndices, pathList, matrixPowerList, lowerLimit, upperLimit, random));
            }
            // Add new chromosomes.
            var bag = new ConcurrentBag<Chromosome>();
            Parallel.For(Chromosomes.Count(), previousPopulation.Chromosomes.Count(), index =>
            {
                // Get a new offspring of two random chromosomes.
                var offspring = previousPopulation.Select(random)
                    .Crossover(previousPopulation.Select(random), nodeIndices, matrixPowerList, nodePreferred, parameters.CrossoverType, random)
                    .Mutate(nodeIndices, pathList, matrixPowerList, nodePreferred, parameters.MutationType, parameters.ProbabilityMutation, random);
                // Add the offspring to the concurrent bag.
                bag.Add(offspring);
            });
            Chromosomes.AddRange(bag);
            // Get the historic best and average fitness.
            HistoricBestFitness = previousPopulation.HistoricBestFitness.Append(GetFitnessList().Max()).ToList();
            HistoricAverageFitness = previousPopulation.HistoricAverageFitness.Append(GetFitnessList().Average()).ToList();
        }

        /// <summary>
        /// Gets the fitness list of the population.
        /// </summary>
        /// <returns></returns>
        public List<double> GetFitnessList()
        {
            // Check if the fitness hasn't already been computed.
            if (_fitnessList == null)
            {
                // Compute the fitness.
                _fitnessList = Chromosomes.Select(item => item.GetFitness()).ToList();
            }
            // Return the fitness.
            return _fitnessList;
        }

        /// <summary>
        /// Gets the combined fitness list of the population, for a Monte-Carlo style selection.
        /// </summary>
        /// <returns></returns>
        public List<double> GetCombinedFitnessList()
        {
            // Check if the fitness list hasn't already been computed.
            if (_fitnessCombinedList == null)
            {
                // Get the fitness of each chromosome.
                var fitness = new List<double> { 0.0 };
                fitness.AddRange(GetFitnessList());
                // Get the total fitness.
                var totalFitness = fitness.Sum();
                // Define a variable to store the temporary sum.
                var sum = 0.0;
                // Compute the combined fitness.
                _fitnessCombinedList = fitness.Select(item => sum += item / totalFitness).ToList();
            }
            // Return the combined fitness.
            return _fitnessCombinedList;
        }

        /// <summary>
        /// Selects a chromosome based on its fitness (the better the fitness, the more chances to be selected).
        /// </summary>
        /// <param name="random">The random seed.</param>
        /// <returns></returns>
        public Chromosome Select(Random random)
        {
            // Get the fitness list.
            var fitnessList = GetCombinedFitnessList();
            // Generate a random value.
            var randomValue = random.NextDouble();
            // Find the index corresponding to the random value.
            var index = fitnessList.FindLastIndex(item => item <= randomValue);
            // Return the chromosome at the specified index.
            return Chromosomes[index];
        }

        /// <summary>
        /// Returns all of the unique chromosomes with the highest fitness in the population (providing an unique combination of genes).
        /// </summary>
        /// <returns></returns>
        public List<Chromosome> GetSolutions()
        {
            // Get the best fitness of the population.
            var bestFitness = GetFitnessList().Max();
            // Define the variable to return.
            var solutions = new List<Chromosome>();
            // Go over all of the chromosomes with the best fitness.
            foreach (var chromosome in Chromosomes.Where(item => item.GetFitness() == bestFitness))
            {
                // Check if the current combination already exists in the list of solutions.
                if (!solutions.Any(item => new HashSet<string>(item.GetUniqueControlNodes()).SetEquals(new HashSet<string>(chromosome.GetUniqueControlNodes()))))
                {
                    // If not, then add it.
                    solutions.Add(chromosome);
                }
            }
            // Return the solutions.
            return solutions;
        }
    }
}
