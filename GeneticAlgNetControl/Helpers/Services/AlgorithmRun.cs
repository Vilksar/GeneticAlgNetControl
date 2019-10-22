using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Services
{
    /// <summary>
    /// Implements a Hangfire task for running an algorithm.
    /// </summary>
    public class AlgorithmRun : IAlgorithmRun
    {
        /// <summary>
        /// Represents the application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public AlgorithmRun(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Runs the algorithm with the given ID.
        /// </summary>
        /// <param name="id">The ID of the analysis to run.</param>
        /// <returns></returns>
        public async Task RunAlgorithm(string id)
        {
            // Load the algorithm with the given ID.
            var algorithm = _context.Algorithms
                .FirstOrDefault(item => item.Id == id);
            // Check if the algorithm hasn't been found.
            if (algorithm == null)
            {
                // End the function.
                return;
            }
            // Get the edges and nodes.
            // ...
        }
    }
}
