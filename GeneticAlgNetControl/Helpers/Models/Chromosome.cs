using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Extensions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents a chromosome used by the algorithm.
    /// </summary>
    public class Chromosome
    {
        /// <summary>
        /// Represents the number of times that each chromosome operations will try to find a valid chromosomes with the given options.
        /// </summary>
        private readonly int _tries = 5;

        /// <summary>
        /// Represents the genes of the chromosome, as a dictionary of a target node to its control node.
        /// </summary>
        public Dictionary<string, string> Genes { get; set; }

        /// <summary>
        /// Constructor for an empty chromosome.
        /// </summary>
        public Chromosome()
        {
            Genes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructor for the chromosome starting from the target nodes.
        /// </summary>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        public Chromosome(List<string> targetNodes)
        {
            // Assign to each gene its default value (the corresponding target node).
            Genes = targetNodes.ToDictionary(item => item, item => item);
        }

        /// <summary>
        /// Computes and gets the unique control nodes in the chromosome.
        /// </summary>
        /// <returns>The unique control nodes in the chromosome.</returns>
        public IEnumerable<string> GetUniqueControlNodes()
        {
            // Return the unique control nodes.
            return Genes.Values.Distinct();
        }

        /// <summary>
        /// Computes and gets the fitness of the chromosome.
        /// </summary>
        /// <returns>The fitness of the chromosome.</returns>
        public double GetFitness()
        {
            // Return the fitness of the chromosome.
            return (double)(Genes.Count() - GetUniqueControlNodes().Count() + 1) * 100 / (double)Genes.Count();
        }

        /// <summary>
        /// Checks if the chromosome is a solution or not.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <returns>True if the chromosome is a solution, false otherwise</returns>
        public bool IsValid(Dictionary<string, int> nodeIndex, List<Matrix<double>> powersMatrixCA)
        {
            // Define the variable to return.
            var isFullRank = false;
            // Get the unique control nodes.
            var uniqueControlNodes = GetUniqueControlNodes().ToList();
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndex.Count(), uniqueControlNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniqueControlNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndex[uniqueControlNodes[index]], index] = 1.0;
            }
            // Initialize the R matrix.
            var matrixR = Matrix<double>.Build.DenseOfMatrix(powersMatrixCA[0]).Multiply(matrixB);
            // Repeat until we reach the maximum power.
            for (int index = 1; index < powersMatrixCA.Count(); index++)
            {
                // Compute the current power matrix.
                matrixR = matrixR.Append(powersMatrixCA[index].Multiply(matrixB));
                // Check if it is full rank.
                isFullRank = matrixR.IsFullRank();
                // Check if it is already full rank.
                if (isFullRank)
                {
                    // Break the loop.
                    break;
                }
            }
            // Return the validity.
            return isFullRank;
        }

        /// <summary>
        /// Gets the actual maximum path length of the solution represented by the chromosome.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <returns>The maximum path length.</returns>
        public int GetMaximumPathLength(Dictionary<string, int> nodeIndex, List<Matrix<double>> powersMatrixCA)
        {
            // Define the variable to return.
            var maximumPathLength = -1;
            // Get the unique control nodes.
            var uniqueControlNodes = GetUniqueControlNodes().ToList();
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndex.Count(), uniqueControlNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniqueControlNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndex[uniqueControlNodes[index]], index] = 1.0;
            }
            // Initialize the R matrix.
            var matrixR = Matrix<double>.Build.DenseOfMatrix(powersMatrixCA[0]).Multiply(matrixB);
            // Repeat until we reach the maximum power.
            for (int index = 1; index < powersMatrixCA.Count(); index++)
            {
                // Compute the current power matrix.
                matrixR = matrixR.Append(powersMatrixCA[index].Multiply(matrixB));
                // Check if it is full rank.
                var isFullRank = matrixR.IsFullRank();
                // Check if it is already full rank.
                if (isFullRank)
                {
                    // Save the maximum path length.
                    maximumPathLength = index;
                    // Break the loop.
                    break;
                }
            }
            // Return the validity.
            return maximumPathLength;
        }

        /// <summary>
        /// Initializes the chromosome with randomly generated gene values between the lower and the upper limit.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetAncestors">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="lowerLimit">The lower limit of the interval in which to randomly generate the values.</param>
        /// <param name="upperLimit">The upper limit of the interval in which to randomly generate the values.</param>
        /// <param name="random">The random seed.</param>
        public Chromosome Initialize(Dictionary<string, int> nodeIndex, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, int lowerLimit, int upperLimit, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Get the genes for which to generate randomly the values.
            var genesRandom = Genes.Keys.ToList().GetRange(lowerLimit, upperLimit - lowerLimit);
            // Repeat while the chromosome is not valid.
            while (genesRandom.Any())
            {
                // Decrease the number of tries.
                tries--;
                // Go over all of the values in the random interval.
                foreach (var item in genesRandom)
                {
                    // Assign a random value from the corresponding list.
                    Genes[item] = targetAncestors[item][random.Next(targetAncestors[item].Count())];
                }
                // Check if the chromosome is valid.
                if (IsValid(nodeIndex, powersMatrixCA))
                {
                    // Exit the loop.
                    break;
                }
                // Check if we reached the last try.
                else if (tries == 0)
                {
                    // Reset the number of tries.
                    tries = 10;
                    // Get a random gene to remove from the list of genes to generate randomly.
                    var randomGene = genesRandom[random.Next(genesRandom.Count())];
                    // Assign to it the default value.
                    Genes[randomGene] = randomGene;
                    // Remove it from the list of genes to generate randomly.
                    genesRandom.Remove(randomGene);
                }
            }
            // Return the chromosome.
            return this;
        }

        /// <summary>
        /// Creates a new offspring chromosome from this and a second chromosome parents.
        /// </summary>
        /// <param name="secondChromosome">The second parent chromosome of the offspring.</param>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, if it is in the preferred node list.</param>
        /// <param name="crossoverType">The crossover type for the algorithm.</param>
        /// <param name="random">The random seed.</param>
        /// <returns></returns>
        public Chromosome Crossover(Chromosome secondChromosome, Dictionary<string, int> nodeIndex, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, AlgorithmCrossoverType crossoverType, Random random)
        {
            // Define a new chromosome.
            var chromosome = new Chromosome(Genes.Keys.ToList());
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Get the number of occurances of each gene in this chromosome and which genes of each are preferred.
            var occurancesInFirst = GetUniqueControlNodes().ToDictionary(item => item, item => Genes.Count(item1 => item1.Value == item));
            var occurancesInSecond = secondChromosome.GetUniqueControlNodes().ToDictionary(item => item, item => Genes.Count(item1 => item1.Value == item));
            // Use the specified crossover type.
            switch (crossoverType)
            {
                // If we have a standard crossover.
                case AlgorithmCrossoverType.Standard:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Get the number of occurances in each chromosome.
                            var inFirst = occurancesInFirst.ContainsKey(Genes[item]) ? occurancesInFirst[Genes[item]] : 0;
                            var inSecond = occurancesInSecond.ContainsKey(secondChromosome.Genes[item]) ? occurancesInSecond[secondChromosome.Genes[item]] : 0;
                            // Assign to the gene in the chromosome its corresponding random parent value with the a probability depending on the occurances.
                            chromosome.Genes[item] = random.NextDouble() < (double)inFirst / (double)(inFirst + inSecond) ? Genes[item] : secondChromosome.Genes[item];
                        }
                        // Check if the chromosome is valid.
                        if (chromosome.IsValid(nodeIndex, powersMatrixCA))
                        {
                            // Exit the loop.
                            break;
                        }
                    }
                    // End the switch statement.
                    break;
                // If we have a crossover with preference.
                case AlgorithmCrossoverType.WithPreference:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Get the number of occurances in each chromosome.
                            var inFirst = occurancesInFirst.ContainsKey(Genes[item]) ? occurancesInFirst[Genes[item]] : 0;
                            var inSecond = occurancesInSecond.ContainsKey(secondChromosome.Genes[item]) ? occurancesInSecond[secondChromosome.Genes[item]] : 0;
                            // Check if the gene in any of the chromosomes is a preferred node.
                            var isPreferredFirst = nodeIsPreferred[Genes[item]];
                            var isPreferredSecond = nodeIsPreferred[secondChromosome.Genes[item]];
                            // Check if the first corresponding gene is preferred, and the second one isn't.
                            if (isPreferredFirst && !isPreferredSecond)
                            {
                                // Keep the first value.
                                //chromosome.Genes[item] = Genes[item];
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)inFirst * 2 / (double)(inFirst * 2 + inSecond) ? Genes[item] : secondChromosome.Genes[item];
                            }
                            // Check if the second corresponding gene is preferred, and the first one isn't.
                            else if (!isPreferredFirst && isPreferredSecond)
                            {
                                // Keep the second value.
                                //chromosome.Genes[item] = secondChromosome.Genes[item];
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)inSecond * 2 / (double)(inFirst + inSecond * 2) ? Genes[item] : secondChromosome.Genes[item];
                            }
                            // Otherwise they both have the same state
                            else
                            {
                                // Choose one of the parent genes with a probability depending on their occurances.
                                chromosome.Genes[item] = random.NextDouble() < (double)inFirst / (double)(inFirst + inSecond) ? Genes[item] : secondChromosome.Genes[item];
                            }
                        }
                        // Check if the chromosome is valid.
                        if (chromosome.IsValid(nodeIndex, powersMatrixCA))
                        {
                            // Exit the loop.
                            break;
                        }
                    }
                    // End the switch statement.
                    break;
                // If we have none of the above.
                default:
                    // Set the tries to 0, such that the new chromosome will inherit completely one of the parents.
                    tries = 0;
                    // End the switch statement.
                    break;
            }
            // Check if the new chromosome is still not valid.
            if (tries == 0)
            {
                // Choose randomly a parent to give all of its genes.
                chromosome.Genes = random.NextDouble() < 0.5 ? Genes : secondChromosome.Genes;
            }
            // Return the new chromosome.
            return chromosome;
        }

        /// <summary>
        /// Mutates the current chromosome based on the given mutation probability.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetAncestors">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, if it is in the preferred node list.</param>
        /// <param name="mutationType">The mutation type for the algorithm.</param>
        /// <param name="mutationProbability">The probability of mutation for any gene of the chromosome.</param>
        /// <param name="random">The random seed.</param>
        public Chromosome Mutate(Dictionary<string, int> nodeIndex, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, AlgorithmMutationType mutationType, double mutationProbability, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Get the genes which will suffer a mutation, together with their current value.
            var genesMutateDictionary = Genes.Where(item => random.NextDouble() < mutationProbability).ToDictionary(item => item.Key, item => item.Value);
            // Use the specified mutation type.
            switch (mutationType)
            {
                // If we have a standard crossover.
                case AlgorithmMutationType.Standard:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys.ToList())
                        {
                            // Assign a random new value from the list.
                            Genes[item] = targetAncestors[item][random.Next(targetAncestors[item].Count())];
                        }
                        // Check if the chromosome is valid.
                        if (IsValid(nodeIndex, powersMatrixCA))
                        {
                            // Exit the loop.
                            break;
                        }
                        // Check if we reached the last try.
                        else if (tries == 0)
                        {
                            // Reset the number of tries.
                            tries = 10;
                            // Get a random gene to remove from the list of genes to mutate.
                            var randomGene = genesMutateDictionary.Keys.ElementAt(random.Next(genesMutateDictionary.Count()));
                            // Assign to it the current value.
                            Genes[randomGene] = genesMutateDictionary[randomGene];
                            // Remove it from the list of genes to mutate.
                            genesMutateDictionary.Remove(randomGene);
                        }
                    }
                    // End the switch statement.
                    break;
                // If we have a mutation with preference.
                case AlgorithmMutationType.WithPreference:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys.ToList())
                        {
                            // Assign a random new value from the list. If it is a preferred node, it it two times less likely to change.
                            Genes[item] = nodeIsPreferred[Genes[item]] && random.NextDouble() < 0.5 ? targetAncestors[item][random.Next(targetAncestors[item].Count())] : Genes[item];
                        }
                        // Check if the chromosome is valid.
                        if (IsValid(nodeIndex, powersMatrixCA))
                        {
                            // Exit the loop.
                            break;
                        }
                        // Check if we reached the last try.
                        else if (tries == 0)
                        {
                            // Reset the number of tries.
                            tries = 10;
                            // Get a random gene to remove from the list of genes to mutate.
                            var randomGene = genesMutateDictionary.Keys.ElementAt(random.Next(genesMutateDictionary.Count()));
                            // Assign to it the current value.
                            Genes[randomGene] = genesMutateDictionary[randomGene];
                            // Remove it from the list of genes to mutate.
                            genesMutateDictionary.Remove(randomGene);
                        }
                    }
                    // End the switch statement.
                    break;
                // If we have none of the above.
                default:
                    // Set the tries to 0, such that the chromosome will be reset.
                    tries = 0;
                    // End the switch statement.
                    break;
            }
            // Check if the new chromosome is still not valid.
            if (genesMutateDictionary.Any())
            {
                // Go over each gene selected for mutation.
                foreach (var item in genesMutateDictionary)
                {
                    // Reset it to the initial values, no mutation taking place.
                    Genes[item.Key] = item.Value;
                }
            }
            // Return the chromosome.
            return this;
        }
    }
}
