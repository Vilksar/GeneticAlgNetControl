using GeneticAlgNetControl.Data.Enumerations;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
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
        /// Represents the chromosomes in the population.
        /// </summary>
        public List<Chromosome> Chromosomes { get; set; }

        /// <summary>
        /// Represents the solutions of the current population.
        /// </summary>
        public List<ChromosomeSolution> Solutions { get; set; }

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
            Solutions = new List<ChromosomeSolution>();
            HistoricBestFitness = new List<double>();
            HistoricAverageFitness = new List<double>();
        }

        /// <summary>
        /// Constructor for the initial population.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <param name="targetAncestors">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="random">The random seed.</param>
        public Population(Dictionary<string, int> nodeIndex, List<string> targetNodes, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, Parameters parameters, Random random)
        {
            // Initialize the list of chromosomes.
            Chromosomes = new List<Chromosome>();
            // Get the number of elements in each group and the minimum number of elements per group.
            var numberOfGroups = (int)Math.Ceiling((double)targetNodes.Count() / (double)parameters.RandomGenesPerChromosome);
            var chromosomesPerGroup = (int)Math.Floor((double)parameters.PopulationSize / (double)numberOfGroups);
            var genesPerGroup = (int)Math.Floor((double)targetNodes.Count() / (double)numberOfGroups);
            var numberOfChromosomeGroupsExtra = parameters.PopulationSize - chromosomesPerGroup * numberOfGroups;
            var numberOfGeneGroupsExtra = targetNodes.Count() - genesPerGroup * numberOfGroups;
            // Get the actual number of chromosomes in each group.
            var chromosomeGroups = new List<int>()
                .Concat(Enumerable.Range(0, numberOfChromosomeGroupsExtra).Select(item => chromosomesPerGroup + 1))
                .Concat(Enumerable.Range(numberOfChromosomeGroupsExtra, numberOfGroups - numberOfChromosomeGroupsExtra).Select(item => chromosomesPerGroup))
                .ToList();
            // Get the actual numer of genes in each group.
            var sum = 0;
            var geneGroups = new List<int> { 0 }
                .Concat(Enumerable.Range(0, numberOfGeneGroupsExtra).Select(item => genesPerGroup + 1))
                .Concat(Enumerable.Range(numberOfGeneGroupsExtra, numberOfGroups - numberOfGeneGroupsExtra).Select(item => genesPerGroup))
                .Select(item => sum += item)
                .ToList();
            // Define a new concurrent bag for chromosomes.
            var bag = new ConcurrentBag<Chromosome>();
            // Define a new thread-safe queue of random seeds, as well as a default random seed.
            var randomSeeds = new ConcurrentQueue<int>(Enumerable.Range(0, parameters.PopulationSize).Select(item => random.Next()));
            var defaultRandomSeed = random.Next();
            // Repeat for each group.
            Parallel.For(0, numberOfGroups, index1 =>
            {
                // Get the lower and upper limits.
                var lowerLimit = geneGroups[index1];
                var upperLimit = geneGroups[index1 + 1];
                // Repeat for the number of elements in the group.
                Parallel.For(0, chromosomeGroups[index1], index2 =>
                {
                    // Try to get a new random seed from the list of random random seeds.
                    if (!randomSeeds.TryDequeue(out var randomSeed))
                    {
                        // If no seed could be gotten, then assign to it the default value.
                        randomSeed = defaultRandomSeed;
                    }
                    // Define a local random variable for only this thread.
                    var localRandom = new Random(randomSeed);
                    // Add a new, initialized, chromosome.
                    bag.Add(new Chromosome(targetNodes).Initialize(nodeIndex, targetAncestors, powersMatrixCA, lowerLimit, upperLimit, localRandom));
                });
            });
            // Add all chromosomes to the current population.
            Chromosomes.AddRange(bag);
            // Get the solutions.
            Solutions = GetSolutions().Select(item => new ChromosomeSolution(item, nodeIndex, nodeIsPreferred, powersMatrixCA)).ToList();
            // Define the historic best and average fitness.
            HistoricBestFitness = new List<double> { GetFitnessList().Max() };
            HistoricAverageFitness = new List<double> { GetFitnessList().Average() };
        }

        /// <summary>
        /// Constructor for a subsequent population.
        /// </summary>
        /// <param name="previousPopulation">The previous population.</param>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <param name="targetAncestors">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, its preferred status.</param>
        /// <param name="parameters">The parameters of the algorithm.</param>
        /// <param name="random">The random seed.</param>
        public Population(Population previousPopulation, Dictionary<string, int> nodeIndex, List<string> targetNodes, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, Parameters parameters, Random random)
        {
            // Initialize the list of chromosomes.
            Chromosomes = new List<Chromosome>();
            // Get the combined fitness list of the population.
            var combinedFitnessList = previousPopulation.GetCombinedFitnessList();
            // Add the specified number of elite chromosomes from the previous population.
            Chromosomes.AddRange(previousPopulation.GetBestChromosomes().OrderBy(item => random.NextDouble()).Take((int)Math.Min((int)Math.Floor(parameters.PercentageElite * parameters.PopulationSize), parameters.PopulationSize)));
            // Define a new thread-safe queue of random seeds, as well as a default random seed.
            var randomSeeds = new ConcurrentQueue<int>(Enumerable.Range(0, parameters.PopulationSize).Select(item => random.Next()));
            var defaultRandomSeed = random.Next();
            // Define a new concurrent bag for chromosomes.
            var bag = new ConcurrentBag<Chromosome>();
            // Add the specified number of random chromosomes.
            Parallel.For(Chromosomes.Count(), (int)Math.Min(Chromosomes.Count() + (int)Math.Floor(parameters.PercentageRandom * parameters.PopulationSize), parameters.PopulationSize), index =>
            {
                // Try to get a new random seed from the list of random random seeds.
                if (!randomSeeds.TryDequeue(out var randomSeed))
                {
                    // If no seed could be gotten, then assign to it the default value.
                    randomSeed = defaultRandomSeed;
                }
                // Define a local random variable for only this thread.
                var localRandom = new Random(randomSeed);
                // Get the lower and upper limits.
                var lowerLimit = localRandom.Next(targetAncestors.Count());
                var upperLimit = (lowerLimit + parameters.RandomGenesPerChromosome) % targetAncestors.Count();
                // Add a new, initialized, chromosome.
                bag.Add(new Chromosome(targetNodes).Initialize(nodeIndex, targetAncestors, powersMatrixCA, lowerLimit, upperLimit, localRandom));
            });
            // Add all chromosomes to the current population.
            Chromosomes.AddRange(bag);
            // Reset the concurrent bag for chromosomes.
            bag = new ConcurrentBag<Chromosome>();
            // Add new chromosomes.
            Parallel.For(Chromosomes.Count(), parameters.PopulationSize, index =>
            {
                // Try to get a new random seed from the list of random random seeds.
                if (!randomSeeds.TryDequeue(out var randomSeed))
                {
                    // If no seed could be gotten, then assign to it the default value.
                    randomSeed = defaultRandomSeed;
                }
                // Define a local random variable for only this thread.
                var localRandom = new Random(randomSeed);
                // Get a new offspring of two random chromosomes.
                var offspring = previousPopulation.Select(combinedFitnessList, localRandom)
                    .Crossover(previousPopulation.Select(combinedFitnessList, localRandom), nodeIndex, powersMatrixCA, nodeIsPreferred, parameters.CrossoverType, localRandom)
                    .Mutate(nodeIndex, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters.MutationType, parameters.ProbabilityMutation, localRandom);
                // Add the offspring to the concurrent bag.
                bag.Add(offspring);
            });
            // Add all chromosomes to the current population.
            Chromosomes.AddRange(bag);
            // Get the solutions.
            Solutions = GetSolutions().Select(item => new ChromosomeSolution(item, nodeIndex, nodeIsPreferred, powersMatrixCA)).ToList();
            // Get the historic best and average fitness.
            HistoricBestFitness = previousPopulation.HistoricBestFitness.Append(GetFitnessList().Max()).ToList();
            HistoricAverageFitness = previousPopulation.HistoricAverageFitness.Append(GetFitnessList().Average()).ToList();
        }

        /// <summary>
        /// Gets the fitness list of the population.
        /// </summary>
        /// <returns>The fitness list.</returns>
        public List<double> GetFitnessList()
        {
            // Return the fitness list.
            return Chromosomes.Select(item => item.GetFitness()).ToList();
        }

        /// <summary>
        /// Gets the combined fitness list of the population, for a Monte-Carlo style selection.
        /// </summary>
        /// <returns>The combined fitness list of the population.</returns>
        public List<double> GetCombinedFitnessList()
        {
            // Get the fitness of each chromosome.
            var fitness = GetFitnessList();
            // Get the total fitness.
            var totalFitness = fitness.Sum();
            // Define a variable to store the temporary sum.
            var sum = 0.0;
            // Return the combined fitness.
            return fitness.Select(item => sum += item).Select(item => item / totalFitness).ToList();
        }

        /// <summary>
        /// Selects a chromosome based on its fitness (the better the fitness, the more chances to be selected).
        /// </summary>
        /// <param name="combinedFitnessList">The combined fitness list for the population.</param>
        /// <param name="random">The random seed.</param>
        /// <returns>A random chromosome, selected based on its fitness.</returns>
        public Chromosome Select(List<double> combinedFitnessList, Random random)
        {
            // Generate a random value.
            var randomValue = random.NextDouble();
            // Find the index corresponding to the random value.
            var index = combinedFitnessList.FindIndex(item => randomValue <= item);
            // Return the chromosome at the specified index.
            return Chromosomes[index];
        }

        /// <summary>
        /// Returns all of the unique chromosomes with the highest fitness in the population (providing an unique combination of genes).
        /// </summary>
        /// <returns>The chromosomes that have the best fitness in the current population.</returns>
        public IEnumerable<Chromosome> GetBestChromosomes()
        {
            // Get the best fitness of the population.
            var bestFitness = GetFitnessList().Max();
            // Define the variable to return.
            var bestChromosomes = new List<Chromosome>();
            // Go over all of the chromosomes with the best fitness.
            foreach (var chromosome in Chromosomes.Where(item => item.GetFitness() == bestFitness))
            {
                // Check if the current combination already exists in the list of solutions.
                if (!bestChromosomes.Any(item => item.IsEqual(chromosome)))
                {
                    // If not, then add it.
                    bestChromosomes.Add(chromosome);
                }
            }
            // Return the solutions.
            return bestChromosomes;
        }

        /// <summary>
        /// Returns all of the unique chromosomes with the highest fitness in the population (providing an unique combination of genes).
        /// </summary>
        /// <returns>The chromosome solutions of the current population.</returns>
        public IEnumerable<Chromosome> GetSolutions()
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
