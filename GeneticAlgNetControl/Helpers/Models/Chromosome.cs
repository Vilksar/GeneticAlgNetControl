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
        /// Represents the fitness of the chromosome.
        /// </summary>
        private double? _fitness = null;

        /// <summary>
        /// Represents the unique control nodes in the chromosome.
        /// </summary>
        private IEnumerable<string> _uniqueControlNodes = null;

        /// <summary>
        /// Represents the genes of the chromosome, as a dictionary of a target node to its control node.
        /// </summary>
        public Dictionary<string, string> Genes { get; set; }

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
        /// Constructor for the chromosome starting from a different chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome from which to build a new one.</param>
        public Chromosome(Chromosome chromosome)
        {
            // Assign to each gene its default value (the corresponding target node).
            Genes = chromosome.Genes.ToDictionary(item => item.Key, item => item.Key);
        }

        /// <summary>
        /// Computes and gets the unique control nodes in the chromosome.
        /// </summary>
        /// <returns>The unique control nodes in the chromosome.</returns>
        public IEnumerable<string> GetUniqueControlNodes()
        {
            // Compute the unique control nodes if they weren't already computed.
            _uniqueControlNodes = _uniqueControlNodes ?? Genes.Values.Distinct();
            // Return them.
            return _uniqueControlNodes;
        }

        /// <summary>
        /// Computes and gets the fitness of the chromosome.
        /// </summary>
        /// <returns>The fitness of the chromosome.</returns>
        public double GetFitness()
        {
            // Compute the fitness if it wasn't already computed.
            _fitness = _fitness ?? (Genes.Count() - GetUniqueControlNodes().Count() + 1) * 100 / Genes.Count();
            // Return it.
            return _fitness.Value;
        }

        /// <summary>
        /// Checks if the chromosome is a solution or not.
        /// </summary>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="matrixPowerList">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <returns>True if the chromosome is a solution, false otherwise</returns>
        public bool IsValid(Dictionary<string, int> nodeIndices, List<Matrix<double>> matrixPowerList)
        {
            // Get the unique control nodes.
            var uniqueControlNodes = Genes.Values.Distinct().ToList();
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndices.Count(), uniqueControlNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniqueControlNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndices[uniqueControlNodes[index]], index] = 1.0;
            }
            // Initialize the R matrix.
            var matrixR = Matrix<Double>.Build.DenseOfMatrix(matrixPowerList[0]).Multiply(matrixB);
            // Repeat until we reach the maximum power.
            for (int index = 1; index < matrixPowerList.Count(); index++)
            {
                // Compute the current power matrix.
                matrixR = matrixR.Append(matrixPowerList[index].Multiply(matrixB));
            }
            // Return the validity.
            return matrixR.IsFullRank();
        }

        /// <summary>
        /// Initializes the chromosome with randomly generated gene values between the lower and the upper limit.
        /// </summary>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="pathList">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="matrixPowerList">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="lowerLimit">The lower limit of the interval in which to randomly generate the values.</param>
        /// <param name="upperLimit">The upper limit of the interval in which to randomly generate the values.</param>
        /// <param name="random">The random seed.</param>
        public Chromosome Initialize(Dictionary<string, int> nodeIndices, Dictionary<string, List<string>> pathList, List<Matrix<double>> matrixPowerList, int lowerLimit, int upperLimit, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = 10;
            // Get the genes for which to generate randomly the values.
            var genesRandom = Genes.Keys.ToList().GetRange(lowerLimit, upperLimit - lowerLimit).ToList();
            // Repeat while the chromosome is not valid.
            while (genesRandom.Any())
            {
                // Decrease the number of tries.
                tries--;
                // Go over all of the values in the random interval.
                foreach (var item in genesRandom)
                {
                    // Assign a random value from the corresponding list.
                    Genes[item] = pathList[item][random.Next(pathList[item].Count())];
                }
                // Check if the chromosome is valid.
                if (IsValid(nodeIndices, matrixPowerList))
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
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="matrixPowerList">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="nodePreferred">The dictionary containing, for each node, if it is in the preferred node list.</param>
        /// <param name="type">The crossover type for the algorithm.</param>
        /// <param name="random">The random seed.</param>
        /// <returns></returns>
        public Chromosome Crossover(Chromosome secondChromosome, Dictionary<string, int> nodeIndices, List<Matrix<double>> matrixPowerList, Dictionary<string, bool> nodePreferred, AlgorithmCrossoverType type, Random random)
        {
            // Define a new chromosome.
            var chromosome = new Chromosome(this);
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = 10;
            // Get the number of occurances of each gene in this chromosome and which genes of each are preferred.
            var occurancesInFirst = GetUniqueControlNodes().ToDictionary(item => item, item => Genes.Count(item1 => item1.Value == item));
            var occurancesInSecond = secondChromosome.GetUniqueControlNodes().ToDictionary(item => item, item => Genes.Count(item1 => item1.Value == item));
            // Use the specified crossover type.
            switch (type)
            {
                // If we have a standard crossover.
                case AlgorithmCrossoverType.Standard:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in Genes.Keys)
                        {
                            // Assign to the gene in the chromosome its corresponding random parent value with the a probability depending on the occurances.
                            chromosome.Genes[item] = random.NextDouble() < (double)occurancesInFirst[Genes[item]] / (double)(occurancesInFirst[Genes[item]] + occurancesInSecond[secondChromosome.Genes[item]]) ? Genes[item] : secondChromosome.Genes[item];
                        }
                        // Check if the chromosome is valid.
                        if (chromosome.IsValid(nodeIndices, matrixPowerList))
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
                        foreach (var item in Genes.Keys)
                        {
                            // Check if the gene in any of the chromosomes is a preferred node.
                            var isPreferredFirst = nodePreferred[Genes[item]];
                            var isPreferredSecond = nodePreferred[secondChromosome.Genes[item]];
                            // Check if the first corresponding gene is preferred, and the second one isn't.
                            if (isPreferredFirst && !isPreferredSecond)
                            {
                                // Keep the first value.
                                //chromosome.Genes[item] = Genes[item];
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)(occurancesInFirst[Genes[item]] * 2) / (double)(occurancesInFirst[Genes[item]] * 2 + occurancesInSecond[secondChromosome.Genes[item]]) ? Genes[item] : secondChromosome.Genes[item];
                            }
                            // Check if the second corresponding gene is preferred, and the first one isn't.
                            else if (!isPreferredFirst && isPreferredSecond)
                            {
                                // Keep the second value.
                                //chromosome.Genes[item] = secondChromosome.Genes[item];
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)(occurancesInSecond[secondChromosome.Genes[item]] * 2) / (double)(occurancesInFirst[Genes[item]] + occurancesInSecond[secondChromosome.Genes[item]] * 2) ? Genes[item] : secondChromosome.Genes[item];
                            }
                            // Otherwise they both have the same state
                            else
                            {
                                // Choose one of the parent genes with a probability depending on their occurances.
                                chromosome.Genes[item] = random.NextDouble() < (double)occurancesInFirst[Genes[item]] / (double)(occurancesInFirst[Genes[item]] + occurancesInSecond[secondChromosome.Genes[item]]) ? Genes[item] : secondChromosome.Genes[item];
                            }
                        }
                        // Check if the chromosome is valid.
                        if (chromosome.IsValid(nodeIndices, matrixPowerList))
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
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="pathList">The list containing, for each target nodes, the nodes from which it can be reached.</param>
        /// <param name="matrixPowerList">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <param name="nodePreferred">The dictionary containing, for each node, if it is in the preferred node list.</param>
        /// <param name="type">The mutation type for the algorithm.</param>
        /// <param name="mutationProbability">The probability of mutation for any gene of the chromosome.</param>
        /// <param name="random">The random seed.</param>
        public Chromosome Mutate(Dictionary<string, int> nodeIndices, Dictionary<string, List<string>> pathList, List<Matrix<double>> matrixPowerList, Dictionary<string, bool> nodePreferred, AlgorithmMutationType type, double mutationProbability, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = 10;
            // Get the genes which will suffer a mutation, together with their current value.
            var genesMutateDictionary = Genes.Where(item => random.NextDouble() < mutationProbability).ToDictionary(item => item.Key, item => item.Value);
            // Use the specified mutation type.
            switch (type)
            {
                // If we have a standard crossover.
                case AlgorithmMutationType.Standard:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys)
                        {
                            // Assign a random new value from the list.
                            Genes[item] = pathList[item][random.Next(pathList[item].Count())];
                        }
                        // Check if the chromosome is valid.
                        if (IsValid(nodeIndices, matrixPowerList))
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
                            var randomGene = genesMutateDictionary[genesMutateDictionary.Keys.ElementAt(random.Next(genesMutateDictionary.Count()))];
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
                        foreach (var item in genesMutateDictionary.Keys)
                        {
                            // Assign a random new value from the list. If it is a preferred node, it it two times less likely to change.
                            Genes[item] = nodePreferred[Genes[item]] && random.NextDouble() < 0.5 ? pathList[item][random.Next(pathList[item].Count())] : Genes[item];
                        }
                        // Check if the chromosome is valid.
                        if (IsValid(nodeIndices, matrixPowerList))
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
                            var randomGene = genesMutateDictionary[genesMutateDictionary.Keys.ElementAt(random.Next(genesMutateDictionary.Count()))];
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
