using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Interfaces
{
    /// <summary>
    /// Provides an abstraction for the algorithm run.
    /// </summary>
    interface IAlgorithmRun
    {
        /// <summary>
        /// Runs the algorithm with the given ID.
        /// </summary>
        /// <param name="id">The ID of the algorithm to run.</param>
        /// <returns></returns>
        Task RunAlgorithm(string id);
    }
}
