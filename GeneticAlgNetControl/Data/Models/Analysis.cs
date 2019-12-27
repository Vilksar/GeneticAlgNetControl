using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data.Models
{
    /// <summary>
    /// Represents the database model of an analysis run.
    /// </summary>
    public class Analysis
    {
        /// <summary>
        /// Represents the unique ID of the analysis in the database.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represents the analysis name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the date and time when the analysis has been started.
        /// </summary>
        public DateTime? DateTimeStarted { get; set; }

        /// <summary>
        /// Represents the date and time when the analysis has ended.
        /// </summary>
        public DateTime? DateTimeEnded { get; set; }

        /// <summary>
        /// Represents the periods of date and time when the analysis was running, with an underlying format of List&lt;DateTimeInterval&gt;.
        /// </summary>
        public string DateTimeIntervals { get; set; }

        /// <summary>
        /// Represents the status of the analysis.
        /// </summary>
        public AnalysisStatus Status { get; set; }

        /// <summary>
        /// Represents the number of edges in the network corresponding to the analysis.
        /// </summary>
        public int NumberOfEdges { get; set; }

        /// <summary>
        /// Represents the edges of the network corresponding to the analysis, with an underlying format of List&lt;Edge&gt;.
        /// </summary>
        public string Edges { get; set; }

        /// <summary>
        /// Represents the number of nodes in the network corresponding to the analysis.
        /// </summary>
        public int NumberOfNodes { get; set; }

        /// <summary>
        /// Represents the nodes of the network corresponding to the analysis, with an underlying format of List&lt;string&gt;.
        /// </summary>
        public string Nodes { get; set; }

        /// <summary>
        /// Represents the number of target nodes in the network corresponding to the analysis.
        /// </summary>
        public int NumberOfTargetNodes { get; set; }

        /// <summary>
        /// Represents the target nodes of the network corresponding to the analysis, with an underlying format of List&lt;string&gt;.
        /// </summary>
        public string TargetNodes { get; set; }

        /// <summary>
        /// Represents the number of preferred nodes in the network corresponding to the analysis.
        /// </summary>
        public int NumberOfPreferredNodes { get; set; }

        /// <summary>
        /// Represents the preferred nodes of the network corresponding to the analysis, with an underlying format of List&lt;string&gt;.
        /// </summary>
        public string PreferredNodes { get; set; }

        /// <summary>
        /// Represents the current iteration of the analysis.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        /// Represents the current iteration without improvement of the analysis.
        /// </summary>
        public int CurrentIterationWithoutImprovement { get; set; }

        /// <summary>
        /// Represents the parameters of the analysis, with an underlying format of Parameters.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Represents the current (last) population of the analysis, with an underlying format of Population.
        /// </summary>
        public string Population { get; set; }

        /// <summary>
        /// Initializes a new default instance of the class.
        /// </summary>
        public Analysis()
        {
            // Assign the default value for each property.
            Id = Guid.NewGuid().ToString();
            Name = string.Empty;
            DateTimeStarted = null;
            DateTimeEnded = null;
            DateTimeIntervals = JsonSerializer.Serialize(new List<DateTimeInterval>());
            Status = AnalysisStatus.Scheduled;
            NumberOfNodes = 0;
            Nodes = JsonSerializer.Serialize(new List<string>());
            NumberOfEdges = 0;
            Edges = JsonSerializer.Serialize(new List<Edge>());
            NumberOfTargetNodes = 0;
            TargetNodes = JsonSerializer.Serialize(new List<string>());
            NumberOfPreferredNodes = 0;
            PreferredNodes = JsonSerializer.Serialize(new List<string>());
            CurrentIteration = 0;
            CurrentIterationWithoutImprovement = 0;
            Parameters = JsonSerializer.Serialize(new Parameters());
            Population = JsonSerializer.Serialize(new Population());
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name of the algorithm.</param>
        /// <param name="edges">The edges of the network corresponding to the algorithm.</param>
        /// <param name="nodes">The nodes of the network corresponding to the algorithm.</param>
        /// <param name="targetNodes">The target nodes of the network corresponding to the algorithm.</param>
        /// <param name="preferredNodes">The preferred nodes of the network corresponding to the algorithm.</param>
        /// <param name="parameters">The parameters of the algorithm.</param>
        public Analysis(string name, IEnumerable<Edge> edges, IEnumerable<string> nodes, IEnumerable<string> targetNodes, IEnumerable<string> preferredNodes, Parameters parameters)
        {
            // Assign the value for each property.
            Id = Guid.NewGuid().ToString();
            Name = name;
            DateTimeStarted = null;
            DateTimeEnded = null;
            DateTimeIntervals = JsonSerializer.Serialize(new List<DateTimeInterval>());
            Status = AnalysisStatus.Scheduled;
            NumberOfNodes = nodes.Count();
            Nodes = JsonSerializer.Serialize(nodes.ToList());
            NumberOfEdges = edges.Count();
            Edges = JsonSerializer.Serialize(edges.ToList());
            NumberOfTargetNodes = targetNodes.Count();
            TargetNodes = JsonSerializer.Serialize(targetNodes.ToList());
            NumberOfPreferredNodes = preferredNodes.Count();
            PreferredNodes = JsonSerializer.Serialize(preferredNodes.ToList());
            CurrentIteration = 0;
            CurrentIterationWithoutImprovement = 0;
            Parameters = JsonSerializer.Serialize(parameters);
            Population = JsonSerializer.Serialize(new Population());
        }

        /// <summary>
        /// Returns a formatted JSON string describing the analysis.
        /// </summary>
        /// <returns>A JSON string which contains all data about the analysis.</returns>
        public string ToJson()
        {
            // Get the analysis related data.
            var dateTimePeriods = JsonSerializer.Deserialize<List<DateTimeInterval>>(DateTimeIntervals);
            var parameters = JsonSerializer.Deserialize<Parameters>(Parameters);
            var population = JsonSerializer.Deserialize<Population>(Population);
            // Define the text to return.
            return JsonSerializer.Serialize(new
            {
                Id = Id,
                Name = Name,
                Status = Status.ToString(),
                CurrentIteration = CurrentIteration,
                CurrentIterationWithoutImprovement = CurrentIterationWithoutImprovement,
                DateTime = new
                {
                    DateTimePeriods = dateTimePeriods,
                    TimeElapsed = dateTimePeriods.Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value)
                },
                Parameters = new
                {
                    Parameters = parameters,
                    CrossoverAlgorithm = parameters.CrossoverType.ToString(),
                    MutationAlgorithm = parameters.MutationType.ToString()
                },
                Solutions = new
                {
                    NumberOfSolutions = population.Solutions.Count(),
                    Solutions = population.Solutions
                },
                HistoricAverageFitness = population.HistoricAverageFitness,
                HistoricBestFitness = population.HistoricBestFitness
            }, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Runs the analysis within the given database context.
        /// </summary>
        /// <param name="context">The database context of the analysis.</param>
        /// <returns></returns>
        public async Task Run(ILogger logger, IHostApplicationLifetime hostApplicationLifetime, ApplicationDbContext context)
        {
            // Log a message.
            logger.LogInformation($"{DateTime.Now.ToString()}: Analysis \"{Name}\" started.");
            // Define a new stopwatch to measure the running time and start it.
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Check if there is a context provided.
            if (context != null)
            {
                // Mark the analysis for updating.
                context.Update(this);
            }
            // Update the analysis status and stats.
            Status = AnalysisStatus.Initializing;
            DateTimeStarted = DateTime.Now;
            DateTimeIntervals = JsonSerializer.Serialize(JsonSerializer.Deserialize<List<DateTimeInterval>>(DateTimeIntervals).Append(new DateTimeInterval(DateTimeStarted, null)));
            // Check if there is a context provided.
            if (context != null)
            {
                // Save the changes in the database.
                await context.SaveChangesAsync();
                // Reload it for a fresh start.
                await context.Entry(this).ReloadAsync();
            }
            // Log a message.
            logger.LogInformation($"{DateTime.Now.ToString()}: Computing the variables needed for the analysis.");
            // Get the edges, nodes, target nodes, preferred nodes and parameters.
            var nodes = JsonSerializer.Deserialize<List<string>>(Nodes);
            var edges = JsonSerializer.Deserialize<List<Edge>>(Edges);
            var targetNodes = JsonSerializer.Deserialize<List<string>>(TargetNodes);
            var preferredNodes = JsonSerializer.Deserialize<List<string>>(PreferredNodes);
            var parameters = JsonSerializer.Deserialize<Parameters>(Parameters);
            // Get the additional needed variables.
            var nodeIndex = Analysis.GetNodeIndex(nodes);
            var nodeIsPreferred = Analysis.GetNodeIsPreferred(nodes, preferredNodes);
            var matrixA = Analysis.GetMatrixA(nodeIndex, edges);
            var matrixC = Analysis.GetMatrixC(nodeIndex, targetNodes);
            var powersMatrixA = Analysis.GetPowersMatrixA(matrixA, parameters.MaximumPathLength);
            var powersMatrixCA = Analysis.GetPowersMatrixCA(matrixC, powersMatrixA);
            var targetAncestors = Analysis.GetTargetAncestors(powersMatrixA, targetNodes, nodeIndex);
            // Update the analysis status.
            Status = AnalysisStatus.Ongoing;
            // Check if there is a context provided.
            if (context != null)
            {
                // Save the changes in the database.
                await context.SaveChangesAsync();
            }
            // Set up the current iteration.
            var random = new Random(parameters.RandomSeed);
            var currentIteration = CurrentIteration;
            var currentIterationWithoutImprovement = CurrentIterationWithoutImprovement;
            var population = JsonSerializer.Deserialize<Population>(Population);
            // Check if the current population is empty.
            if (!population.Chromosomes.Any())
            {
                // Log a message.
                logger.LogInformation($"{DateTime.Now.ToString()}: Setting up the first population.");
                // Initialize a new population.
                population = new Population(nodeIndex, targetNodes, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters, random);
            }
            // Get the best fitness so far.
            var bestFitness = population.HistoricBestFitness.Max();
            // Log a message.
            logger.LogInformation($"{DateTime.Now.ToString()}:\t{currentIteration}\t/\t{parameters.MaximumIterations}\t|\t{currentIterationWithoutImprovement}\t/\t{parameters.MaximumIterationsWithoutImprovement}\t|\t{bestFitness}\t|\t{population.HistoricAverageFitness.Last()}");
            // Move through the generations.
            while (!hostApplicationLifetime.ApplicationStopping.IsCancellationRequested && this != null && Status == AnalysisStatus.Ongoing && currentIteration < parameters.MaximumIterations && currentIterationWithoutImprovement < parameters.MaximumIterationsWithoutImprovement)
            {
                // Move on to the next iterations.
                currentIteration += 1;
                currentIterationWithoutImprovement += 1;
                // Update the iteration count.
                CurrentIteration = currentIteration;
                CurrentIterationWithoutImprovement = currentIterationWithoutImprovement;
                // Check if there is a context provided.
                if (context != null)
                {
                    // Save the changes in the database.
                    await context.SaveChangesAsync();
                }
                // Move on to the next population.
                population = new Population(population, nodeIndex, targetNodes, targetAncestors, powersMatrixCA, nodeIsPreferred, parameters, random);
                // Get the best fitness of the current population.
                var fitness = population.HistoricBestFitness.Last();
                // Check if the current solution is better than the previous solution.
                if (bestFitness < fitness)
                {
                    // Update the fitness.
                    bestFitness = fitness;
                    currentIterationWithoutImprovement = 0;
                }
                // Log a message.
                logger.LogInformation($"{DateTime.Now.ToString()}:\t{currentIteration}\t/\t{parameters.MaximumIterations}\t|\t{currentIterationWithoutImprovement}\t/\t{parameters.MaximumIterationsWithoutImprovement}\t|\t{bestFitness}\t|\t{population.HistoricAverageFitness.Last()}");
                // Check if there is a context provided.
                if (context != null)
                {
                    // Reload it for a fresh start.
                    await context.Entry(this).ReloadAsync();
                }
            }
            // Check if the analysis doesn't exist anymore (if it has been deleted).
            if (this == null)
            {
                // End the function.
                return;
            }
            // Update the solutions, end time and the status.
            Population = JsonSerializer.Serialize(population);
            Status = Status == AnalysisStatus.Stopping ? AnalysisStatus.Stopped : AnalysisStatus.Completed;
            DateTimeEnded = DateTime.Now;
            DateTimeIntervals = JsonSerializer.Serialize(JsonSerializer.Deserialize<List<DateTimeInterval>>(DateTimeIntervals).SkipLast(1).Append(new DateTimeInterval(DateTimeStarted, DateTimeEnded)));
            // Check if the application is stopping, but the analysis has not ended.
            if (hostApplicationLifetime.ApplicationStopping.IsCancellationRequested && currentIteration < parameters.MaximumIterations && currentIterationWithoutImprovement < parameters.MaximumIterationsWithoutImprovement)
            {
                // Update the status such that it will start automatically upon the next application launch.
                Status = AnalysisStatus.Ongoing;
            }
            // Check if there is a context provided.
            if (context != null)
            {
                // Save the changes in the database.
                await context.SaveChangesAsync();
            }
            // Stop the measuring watch.
            stopwatch.Stop();
            // Log a message.
            logger.LogInformation($"{DateTime.Now.ToString()}: Analysis ended in {stopwatch.Elapsed}.");
            // End the function.
            return;
        }

        /// <summary>
        /// Represents a date and time interval in which the analysis runs (used instead of Tuple&lt;DateTime?, DateTime?&gt;).
        /// </summary>
        public class DateTimeInterval
        {
            /// <summary>
            /// Represents the start time of the interval.
            /// </summary>
            public DateTime? DateTimeStarted { get; set; }

            /// <summary>
            /// Represents the end time of the interval.
            /// </summary>
            public DateTime? DateTimeEnded { get; set; }

            /// <summary>
            /// Initializes a new default instance of the class.
            /// </summary>
            public DateTimeInterval()
            {
                // Assign the default value for each property.
                DateTimeStarted = null;
                DateTimeEnded = null;
            }

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="dateTimeStarted">The start time of the interval.</param>
            /// <param name="dateTimeEnded">The end time of the interval.</param>
            public DateTimeInterval(DateTime? dateTimeStarted, DateTime? dateTimeEnded)
            {
                // Assign the value for each property.
                DateTimeStarted = dateTimeStarted;
                DateTimeEnded = dateTimeEnded;
            }
        }

        /// <summary>
        /// Represents an edge in the network (used instead of Tuple&lt;string, string&gt;).
        /// </summary>
        public class Edge
        {
            /// <summary>
            /// Represents the source node of the edge.
            /// </summary>
            public string SourceNode { get; set; }

            /// <summary>
            /// Represents the target node of the edge.
            /// </summary>
            public string TargetNode { get; set; }

            /// <summary>
            /// Initializes a new default instance of the class.
            /// </summary>
            public Edge()
            {
                // Assign the default value for each property.
                SourceNode = null;
                TargetNode = null;
            }

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="sourceNode"></param>
            /// <param name="targetNode"></param>
            public Edge(string sourceNode, string targetNode)
            {
                // Assign the value for each property.
                SourceNode = sourceNode;
                TargetNode = targetNode;
            }
        }

        /// <summary>
        /// Gets the dictionary containing, for each node, its index in the node list, for faster reference.
        /// </summary>
        /// <param name="nodes">The nodes of the graph.</param>
        /// <returns>The dictionary containing, for each node, its index in the node list, for faster reference.</returns>
        public static Dictionary<string, int> GetNodeIndex(List<string> nodes)
        {
            // Return the dictionary for nodes and their indices.
            return nodes.Select((item, index) => (item, index)).ToDictionary(item => item.item, item => item.index);
        }

        /// <summary>
        /// Gets the dictionary containing, for each node, its preferred status, for faster reference.
        /// </summary>
        /// <param name="nodes">The nodes of the graph.</param>
        /// <param name="preferredNodes">The preferred nodes of the graph.</param>
        /// <returns>The dictionary containing, for each node, its preferred status, for faster reference.</returns>
        public static Dictionary<string, bool> GetNodeIsPreferred(List<string> nodes, List<string> preferredNodes)
        {
            // Return the dictionary for nodes and preferred status.
            return nodes.ToDictionary(item => item, item => preferredNodes.Contains(item));
        }

        /// <summary>
        /// Computes the A matrix (corresponding to the adjacency matrix).
        /// </summary>
        /// <param name="nodeIndices">The dictionary containing, for each node, its index in the node list.</param>
        /// <param name="edges">The edges of the graph.</param>
        /// <returns>The A matrix (corresponding to the adjacency matrix).</returns>
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
        /// <returns>The C matrix (corresponding to the target nodes).</returns>
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
        /// <param name="maximumPathLength">The maximum path length for control in the graph.</param>
        /// <returns>The powers of the adjacency matrix A, up to a given maximum power.</returns>
        public static List<Matrix<double>> GetPowersMatrixA(Matrix<double> matrixA, int maximumPathLength)
        {
            // Initialize a matrix list with the identity matrix.
            var powers = new List<Matrix<double>>(maximumPathLength + 1)
            {
                Matrix<double>.Build.DenseIdentity(matrixA.RowCount)
            };
            // Up to the maximum power, starting from the first element.
            for (int index = 1; index < maximumPathLength + 1; index++)
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
        /// <param name="powersMatrixA">The list of powers of the adjacency matrix A.</param>
        /// <returns>The powers of the combination between the target matrix C and the adjacency matrix A.</returns>
        public static List<Matrix<double>> GetPowersMatrixCA(Matrix<double> matrixC, List<Matrix<double>> powersMatrixA)
        {
            // Initialize a new empty list.
            var powers = new List<Matrix<double>>(powersMatrixA.Count());
            // Go over each power of the adjacency matrix.
            foreach (var power in powersMatrixA)
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
        /// <param name="powersMatrixA">The list of powers of the adjacency matrix A.</param>
        /// <param name="targetNodes">The target nodes for the algorithm.</param>
        /// <param name="nodeIndex">The dictionary containing, for each node, its index in the node list.</param>
        /// <returns>The list of nodes from which every target node can be reached.</returns>
        public static Dictionary<string, List<string>> GetTargetAncestors(List<Matrix<double>> powersMatrixA, List<string> targetNodes, Dictionary<string, int> nodeIndex)
        {
            // Initialize the path dictionary with an empty list for each target node.
            var dictionary = targetNodes.ToDictionary(item => item, item => new List<string>());
            // For every power of the adjacency matrix.
            for (int index1 = 0; index1 < powersMatrixA.Count(); index1++)
            {
                // For every target node.
                for (int index2 = 0; index2 < targetNodes.Count(); index2++)
                {
                    // Add to the target node all of the nodes corresponding to the non-zero entries in the proper row of the matrix.
                    dictionary[targetNodes[index2]].AddRange(powersMatrixA[index1]
                        .Row(nodeIndex[targetNodes[index2]])
                        .Select((value, index) => value != 0 ? nodeIndex.FirstOrDefault(item => item.Value == index).Key : null)
                        .Where(item => !string.IsNullOrEmpty(item))
                        .ToList());
                }
            }
            // For each item in the dictionary.
            foreach (var item in dictionary.Keys.ToList())
            {
                // Remove all duplicate nodes.
                dictionary[item] = dictionary[item].Distinct().ToList();
            }
            // Return the dictionary.
            return dictionary;
        }
    }
}
