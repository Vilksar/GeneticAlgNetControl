﻿using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Extensions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgNetControl.Helpers.Models
{
    /// <summary>
    /// Represents a chromosome used by the analysis.
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
        /// Compares the current chromosome with a new one.
        /// </summary>
        /// <param name="chromosome">The chromosome to compare with.</param>
        /// <returns>True if both chromosomes have the same values for all genes, false otherwise.</returns>
        public bool IsEqual(Chromosome chromosome)
        {
            // Define the variable to return.
            var isEqual = true;
            // Go over each gene.
            foreach (var item in Genes.Keys.ToList())
            {
                // Compare them.
                if (Genes[item] != chromosome.Genes[item])
                {
                    // Mark them as not equal.
                    isEqual = false;
                    // Break the loop.
                    break;
                }
            }
            // Return the equality status.
            return isEqual;
        }

        /// <summary>
        /// Computes and gets the unique input nodes in the chromosome.
        /// </summary>
        /// <returns>The unique input nodes in the chromosome.</returns>
        public IEnumerable<string> GetUniqueInputNodes()
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
            return (double)(Genes.Count() - GetUniqueInputNodes().Count() + 1) * 100 / (double)Genes.Count();
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
            var uniqueInputNodes = GetUniqueInputNodes().ToList();
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndex.Count(), uniqueInputNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniqueInputNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndex[uniqueInputNodes[index]], index] = 1.0;
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
            var uniqueInputNodes = GetUniqueInputNodes().ToList();
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndex.Count(), uniqueInputNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniqueInputNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndex[uniqueInputNodes[index]], index] = 1.0;
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
        /// Gets the number of target nodes that can be controlled only by the preferred nodes in the solution.
        /// </summary>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="nodeIsPreferred">The dictionary containing, for each node, if it is in the preferred node list.</param>
        /// <param name="powersMatrixCA">The list containing the different powers of the matrix (CA, CA^2, CA^3, ... ).</param>
        /// <returns>The number of target nodes that can be controlled only by the preferred nodes in the solution.</returns>
        public int GetNumberOfTargetsControlledByPreferredInputNodes(Dictionary<string, int> nodeIndex, Dictionary<string, bool> nodeIsPreferred, List<Matrix<double>> powersMatrixCA)
        {
            // Get the unique preferred input nodes.
            var uniquePreferredInputNodes = GetUniqueInputNodes().Where(item => nodeIsPreferred[item]).ToList();
            // Check if there aren't any preferred nodes.
            if (!uniquePreferredInputNodes.Any())
            {
                // Return zero.
                return 0;
            }
            // Initialize the B matrix.
            var matrixB = Matrix<double>.Build.Dense(nodeIndex.Count(), uniquePreferredInputNodes.Count());
            // Go over each control node.
            for (int index = 0; index < uniquePreferredInputNodes.Count(); index++)
            {
                // Update the corresponding field.
                matrixB[nodeIndex[uniquePreferredInputNodes[index]], index] = 1.0;
            }
            // Initialize the R matrix.
            var matrixR = Matrix<double>.Build.DenseOfMatrix(powersMatrixCA[0]).Multiply(matrixB);
            // Repeat until we reach the maximum power.
            for (int index = 1; index < powersMatrixCA.Count(); index++)
            {
                // Compute the current power matrix.
                matrixR = matrixR.Append(powersMatrixCA[index].Multiply(matrixB));
            }
            // Return the rank of the matrix.
            return matrixR.Rank();
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
        /// <returns>The same chromosome, with randomly generated gene values between the lower and the upper limit.</returns>
        public Chromosome Initialize(Dictionary<string, int> nodeIndex, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, int lowerLimit, int upperLimit, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Start from all of the target genes.
            var genesRandom = Genes.Keys.ToList();
            // Check if the lower limit is smaller than the upper limit.
            if (lowerLimit < upperLimit)
            {
                // Get the genes for which to generate randomly the values.
                genesRandom = genesRandom.GetRange(lowerLimit, upperLimit - lowerLimit);
            }
            else
            {
                // Get the genes for which to generate randomly the values.
                genesRandom = genesRandom.GetRange(0, upperLimit).Concat(genesRandom.GetRange(lowerLimit, genesRandom.Count() - lowerLimit)).ToList();
            }
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
                    tries = _tries;
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
        /// <returns>A new offspring chromosome from this and a second chromosome parents.</returns>
        public Chromosome Crossover(Chromosome secondChromosome, Dictionary<string, int> nodeIndex, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, AnalysisCrossoverType crossoverType, Random random)
        {
            // Define a new chromosome.
            var chromosome = new Chromosome(Genes.Keys.ToList());
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Get the number of occurances of each gene in this chromosome and which genes of each are preferred.
            var occurrences = GetUniqueInputNodes()
                .Concat(secondChromosome.GetUniqueInputNodes())
                .Distinct()
                .ToDictionary(item => item, item => Genes.Count(item1 => item1.Value == item) + secondChromosome.Genes.Count(item1 => item1.Value == item));
            // Use the specified crossover type.
            switch (crossoverType)
            {
                // If we have a standard crossover.
                case AnalysisCrossoverType.WeightedRandom:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Get the number of occurances in each chromosome.
                            var inFirst = occurrences[Genes[item]];
                            var inSecond = occurrences[secondChromosome.Genes[item]];
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
                // If we have a standard crossover with preference.
                case AnalysisCrossoverType.WeightedRandomPreferred:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Get the number of occurances in each chromosome.
                            var inFirst = occurrences[Genes[item]];
                            var inSecond = occurrences[secondChromosome.Genes[item]];
                            // Check if the gene in any of the chromosomes is a preferred node.
                            var isPreferredFirst = nodeIsPreferred[Genes[item]];
                            var isPreferredSecond = nodeIsPreferred[secondChromosome.Genes[item]];
                            // Check if the first corresponding gene is preferred, and the second one isn't.
                            if (isPreferredFirst && !isPreferredSecond)
                            {
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)inFirst * 2 / (double)(inFirst * 2 + inSecond) ? Genes[item] : secondChromosome.Genes[item];
                            }
                            // Check if the second corresponding gene is preferred, and the first one isn't.
                            else if (!isPreferredFirst && isPreferredSecond)
                            {
                                // Choose one of the parent genes with a probability depending on their occurances, the preferred node being two times more likely to be selected.
                                chromosome.Genes[item] = random.NextDouble() < (double)inSecond * 2 / (double)(inFirst + inSecond * 2) ? secondChromosome.Genes[item] : Genes[item];
                            }
                            // Otherwise they both have the same state.
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
                // If we have a default crossover.
                case AnalysisCrossoverType.Dominant:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Get the number of occurances in each chromosome.
                            var inFirst = occurrences[Genes[item]];
                            var inSecond = occurrences[secondChromosome.Genes[item]];
                            // Assign to the gene in the chromosome its corresponding random parent value with the a probability depending on the occurances.
                            chromosome.Genes[item] = inSecond < inFirst ? Genes[item] : inFirst < inSecond ? secondChromosome.Genes[item] : random.NextDouble() < 0.5 ? Genes[item] : secondChromosome.Genes[item];
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
                // If we have a default crossover with preference.
                case AnalysisCrossoverType.DominantPreferred:
                    // Repeat while the chromosome is not valid.
                    while (tries > 0)
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over each of the target nodes.
                        foreach (var item in chromosome.Genes.Keys.ToList())
                        {
                            // Check if the gene in any of the chromosomes is a preferred node.
                            var isPreferredFirst = nodeIsPreferred[Genes[item]];
                            var isPreferredSecond = nodeIsPreferred[secondChromosome.Genes[item]];
                            // Check if the first corresponding gene is preferred, and the second one isn't.
                            if (isPreferredFirst && !isPreferredSecond)
                            {
                                // Choose the preferred node.
                                chromosome.Genes[item] = Genes[item];
                            }
                            // Check if the second corresponding gene is preferred, and the first one isn't.
                            else if (!isPreferredFirst && isPreferredSecond)
                            {
                                // Choose the preferred node.
                                chromosome.Genes[item] = secondChromosome.Genes[item];
                            }
                            // Otherwise they both have the same state.
                            else
                            {
                                // Get the number of occurances in each chromosome.
                                var inFirst = occurrences[Genes[item]];
                                var inSecond = occurrences[secondChromosome.Genes[item]];
                                // Choose one of the parent genes with a probability depending on their occurances.
                                chromosome.Genes[item] = inSecond < inFirst ? Genes[item] : inFirst < inSecond ? secondChromosome.Genes[item] : random.NextDouble() < 0.5 ? Genes[item] : secondChromosome.Genes[item];
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
                chromosome.Genes = random.NextDouble() < 0.5 ? new Dictionary<string, string>(Genes) : new Dictionary<string, string>(secondChromosome.Genes);
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
        /// <returns>The same chromosome, with the mutation operator applied on a random number of its genes.</returns>
        public Chromosome Mutate(Dictionary<string, int> nodeIndex, Dictionary<string, List<string>> targetAncestors, List<Matrix<double>> powersMatrixCA, Dictionary<string, bool> nodeIsPreferred, AnalysisMutationType mutationType, double mutationProbability, Random random)
        {
            // Define the number of tries in which to try and find a valid chromosome.
            var tries = _tries;
            // Get the genes which will suffer a mutation, together with their current value.
            var genesMutateDictionary = Genes.Where(item => random.NextDouble() < mutationProbability).ToDictionary(item => item.Key, item => item.Value);
            // Use the specified mutation type.
            switch (mutationType)
            {
                // If we have a standard mutation.
                case AnalysisMutationType.WeightedRandom:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys.ToList())
                        {
                            // Define a random value based on which to select a new value for the mutated gene.
                            var randomValue = random.NextDouble();
                            // Get the number of occurences of each ancestor in the current chromosome and select a gene based on the number of occurences.
                            var occurences = targetAncestors[item].ToDictionary(item1 => item1, item1 => Genes.Values.Count(item2 => item2 == item1) + 1);
                            // Get the variables required for finding a random index based on the number of occurences.
                            var totalOccurences = occurences.Values.Sum();
                            var sum = 0.0;
                            var randomIndex = occurences.Values.Select(item1 => sum += item1).Select(item1 => item1 / totalOccurences).ToList().FindIndex(item1 => randomValue <= item1);
                            // Assign a random new value from the list.
                            Genes[item] = occurences.ElementAt(randomIndex).Key;
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
                            tries = _tries;
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
                // If we have a stadard mutation with preference.
                case AnalysisMutationType.WeightedRandomPreferred:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys.ToList())
                        {
                            // Define a random value based on which to select a new value for the mutated gene.
                            var randomValue = random.NextDouble();
                            // Get the number of occurences of each ancestor in the current chromosome and select a gene based on the number of occurences. If the ancestor is preferred, then it is twice more likely to be selected.
                            var occurences = targetAncestors[item].ToDictionary(item1 => item1, item1 => Genes.Values.Count(item2 => item2 == item1) + 1).ToDictionary(item1 => item1.Key, item1 => nodeIsPreferred[item1.Key] ? 2 * item1.Value : item1.Value);
                            // Get the variables required for finding a random index based on the number of occurences.
                            var totalOccurences = occurences.Values.Sum();
                            var sum = 0.0;
                            var randomIndex = occurences.Values.Select(item1 => sum += item1).Select(item1 => item1 / totalOccurences).ToList().FindIndex(item1 => randomValue <= item1);
                            // Assign a random new value from the list.
                            Genes[item] = occurences.ElementAt(randomIndex).Key;
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
                            tries = _tries;
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
                // If we have a default mutation.
                case AnalysisMutationType.Random:
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
                            tries = _tries;
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
                // If we have a default mutation with preference.
                case AnalysisMutationType.RandomPreferred:
                    // Repeat while the chromosome is not valid.
                    while (genesMutateDictionary.Any())
                    {
                        // Decrease the number of tries.
                        tries--;
                        // Go over all of the values in the genes to mutate.
                        foreach (var item in genesMutateDictionary.Keys.ToList())
                        {
                            // Get all of the ancestors that are preferred.
                            var preferredAncestors = targetAncestors[item].Where(item1 => nodeIsPreferred[item1]).ToList();
                            // Assign a random new value from the list. If any of them are preferred, select a preferred ancestor randomly.
                            Genes[item] = preferredAncestors.Any() ? preferredAncestors[random.Next(preferredAncestors.Count())] : targetAncestors[item][random.Next(targetAncestors[item].Count())];
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
                            tries = _tries;
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
                    // End the switch statement.
                    break;
            }
            // Return the chromosome.
            return this;
        }
    }
}
