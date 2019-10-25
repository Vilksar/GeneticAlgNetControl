using GeneticAlgNetControl.Helpers.Models;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Functions
{
    /// <summary>
    /// Defines several functions used by the algorithm.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Gets the dictionary containing, for each node, its index in the node list, for faster reference.
        /// </summary>
        /// <param name="nodes">The nodes of the graph.</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetNodeIndices(List<string> nodes)
        {
            // Return the dictionary for nodes and their indices.
            return nodes.Select((item, index) => (item, index)).ToDictionary(item => item.item, item => item.index);
        }

        /// <summary>
        /// Gets the dictionary containing, for each node, its preferred status, for faster reference.
        /// </summary>
        /// <param name="nodes">The nodes of the graph.</param>
        /// <param name="preferredNodes">The preferred nodes of the graph.</param>
        /// <returns></returns>
        public static Dictionary<string, bool> GetNodePreferred(List<string> nodes, List<string> preferredNodes)
        {
            // Return the dictionary for nodes and preferred status.
            return nodes.ToDictionary(item => item, item => preferredNodes.Contains(item));
        }

        /// <summary>
        /// Computes the A matrix (corresponding to the adjacency matrix).
        /// </summary>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="edges">The edges of the graph.</param>
        /// <returns></returns>
        public static Matrix<double> GetMatrixA(Dictionary<string, int> nodeIndices, List<Edge> edges)
        {
            // Initialize the adjacency matrix with zero.
            var matrixA = Matrix<double>.Build.DenseDiagonal(nodeIndices.Count(), nodeIndices.Count(), 0.0);
            // Go over each of the edges.
            foreach (var edge in edges)
            {
                // Set to 1.0 the corresponding entry in the matrix (source nodes are on the columns, target nodes are on the rows).
                matrixA[nodeIndices[edge.TargetNode], nodeIndices[edge.SourceNode]] = 1.0;
            }
            // Return the matrix.
            return matrixA;
        }

        /// <summary>
        /// Computes the C matrix (corresponding to the target nodes).
        /// </summary>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <returns></returns>
        public static Matrix<double> GetMatrixC(Dictionary<string, int> nodeIndices, List<string> targetNodes)
        {
            // Initialize the C matrix with zero.
            var matrixC = Matrix<double>.Build.Dense(targetNodes.Count(), nodeIndices.Count());
            // Go over each target node,
            for (int index = 0; index < targetNodes.Count(); index++)
            {
                // Set to 1.0 the corresponding entry in the matrix.
                matrixC[index, nodeIndices[targetNodes[index]]] = 1.0;
            }
            // And we return the matrix.
            return matrixC;
        }

        /// <summary>
        /// Computes the powers of the adjacency matrix A, up to a given maximum power.
        /// </summary>
        /// <param name="matrixA">The adjacency matrix of the graph.</param>
        /// <param name="maximumPower">The maximum path length for control in the graph.</param>
        /// <returns></returns>
        public static List<Matrix<double>> GetAdjacencyMatrixPowers(Matrix<double> matrixA, int maximumPower)
        {
            // Initialize a matrix list with the identity matrix.
            var powers = new List<Matrix<double>>(maximumPower + 1)
            {
                Matrix<double>.Build.DenseIdentity(matrixA.RowCount)
            };
            // Up to the maximum power, starting from the first element.
            for (int index = 1; index < maximumPower + 1; index++)
            {
                // Multiply the previous element with the matrix itself.
                powers.Add(matrixA.Multiply(powers[index - 1]));
            }
            // Return the list.
            return powers;
        }

        /// <summary>
        /// Computes the powers of the combination between the target matrix C and the adjacency matrix A.
        /// </summary>
        /// <param name="matrixC">The matrix corresponding to the target nodes in the graph.</param>
        /// <param name="adjacencyPowers">The list of powers of the adjacency matrix A.</param>
        /// <returns></returns>
        public static List<Matrix<double>> GetMatrixPowers(Matrix<double> matrixC, List<Matrix<double>> adjacencyPowers)
        {
            // Initialize a new empty list.
            var powers = new List<Matrix<double>>(adjacencyPowers.Count());
            // Go over each power of the adjacency matrix.
            foreach (var power in adjacencyPowers)
            {
                // Left-multiply with the target matrix C.
                powers.Add(matrixC.Multiply(power));
            }
            // Return the list.
            return powers;
        }

        /// <summary>
        /// Computes, for every taret node, the list of nodes from which it can be reached.
        /// </summary>
        /// <param name="adjacencyPowers">The list of powers of the adjacency matrix A.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetPathList(List<Matrix<double>> adjacencyPowers, List<string> targetNodes, Dictionary<string, int> nodeIndices)
        {
            // Initialize the path dictionary with an empty list for each target node.
            var dictionary = targetNodes.ToDictionary(item => item, item => new List<string>());
            // For every power of the adjacency matrix.
            for (int index = 0; index < adjacencyPowers.Count(); index++)
            {
                // Add to the target node all of the nodes corresponding to the non-zero entries in the proper row of the matrix.
                dictionary[targetNodes[index]].AddRange(adjacencyPowers[index]
                    .Row(nodeIndices[targetNodes[index]])
                    .Select((value, index) => value != 0 ? nodeIndices.FirstOrDefault(item => item.Value == value).Key : null)
                    .Where(item => !string.IsNullOrEmpty(item))
                    .ToList());
            }
            // For each item in the dictionary.
            foreach (var item in dictionary.Keys)
            {
                // Remove all duplicate nodes.
                dictionary[item] = dictionary[item].Distinct().ToList();
            }
            // Return the dictionary.
            return dictionary;
        }
    }
}
