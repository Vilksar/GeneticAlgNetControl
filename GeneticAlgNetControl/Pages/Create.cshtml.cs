using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Extensions;
using GeneticAlgNetControl.Helpers.Models;
using GeneticAlgNetControl.Helpers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public CreateModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : IValidatableObject
        {
            [Required(ErrorMessage = "This field is required.")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "This field is required.")]
            public string Edges { get; set; } = JsonSerializer.Serialize(Enumerable.Empty<(string, string)>());

            [Required(ErrorMessage = "This field is required.")]
            public string TargetNodes { get; set; } = JsonSerializer.Serialize(Enumerable.Empty<string>());

            [Required(ErrorMessage = "This field is required.")]
            public string PreferredNodes { get; set; } = JsonSerializer.Serialize(Enumerable.Empty<string>());

            [Required(ErrorMessage = "This field is required.")]
            [Range(0, int.MaxValue, ErrorMessage = "The value must be a positive integer.")]
            public int RandomSeed { get; set; } = Parameters.DefaultValues.RandomSeed;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumIterations { get; set; } = Parameters.DefaultValues.MaximumIterations;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumIterationsWithoutImprovement { get; set; } = Parameters.DefaultValues.MaximumIterationsWithoutImprovement;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumPathLength { get; set; } = Parameters.DefaultValues.MaximumPathLength;

            [Required(ErrorMessage = "This field is required.")]
            [Range(2, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int PopulationSize { get; set; } = Parameters.DefaultValues.PopulationSize;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int RandomGenesPerChromosome { get; set; } = Parameters.DefaultValues.RandomGenesPerChromosome;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double PercentageRandom { get; set; } = Parameters.DefaultValues.PercentageRandom;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double PercentageElite { get; set; } = Parameters.DefaultValues.PercentageElite;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double ProbabilityMutation { get; set; } = Parameters.DefaultValues.ProbabilityMutation;

            [Required(ErrorMessage = "This field is required.")]
            public AlgorithmCrossoverType CrossoverType { get; set; }

            [Required(ErrorMessage = "This field is required.")]
            public AlgorithmMutationType MutationType { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                // Check if the string is a valid JSON object.
                if (Edges.IsNullOrInvalidJsonObject<IEnumerable<(string, string)>>())
                {
                    yield return new ValidationResult("The value is not a valid JSON string.", new List<string> { nameof(Edges) });
                }
                // Check if the string is a valid JSON object.
                if (TargetNodes.IsNullOrInvalidJsonObject<IEnumerable<string>>())
                {
                    yield return new ValidationResult("The value is not a valid JSON string.", new List<string> { nameof(TargetNodes) });
                }
                // Check if the string is a valid JSON object.
                if (PreferredNodes.IsNullOrInvalidJsonObject<IEnumerable<string>>())
                {
                    yield return new ValidationResult("The value is not a valid JSON string.", new List<string> { nameof(PreferredNodes) });
                }
            }
        }

        public IActionResult OnGet(string id = null)
        {
            // Check if there isn't any ID provided.
            if (string.IsNullOrEmpty(id))
            {
                // Define the input.
                Input = new InputModel();
                // Return the page.
                return Page();
            }
            // Try to get the item with the given ID.
            var algorithm = _context.Algorithms
                .FirstOrDefault(item => item.Id == id);
            // Check if there was no item found.
            if (algorithm == null)
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: There is no algorithm with the specified ID.";
                // Redirect to the dashboard page.
                return RedirectToPage("/Dashboard");
            }
            // Define the input.
            Input = new InputModel
            {
                Name = algorithm.Name,
                Edges = JsonSerializer.Serialize(algorithm.Edges),
                TargetNodes = JsonSerializer.Serialize(algorithm.TargetNodes),
                PreferredNodes = JsonSerializer.Serialize(algorithm.PreferredNodes),
                RandomSeed = algorithm.Parameters.RandomSeed,
                MaximumIterations = algorithm.Parameters.MaximumIterations,
                MaximumIterationsWithoutImprovement = algorithm.Parameters.MaximumIterationsWithoutImprovement,
                MaximumPathLength = algorithm.Parameters.MaximumPathLength,
                PopulationSize = algorithm.Parameters.PopulationSize,
                RandomGenesPerChromosome = algorithm.Parameters.RandomGenesPerChromosome,
                PercentageRandom = algorithm.Parameters.PercentageRandom,
                PercentageElite = algorithm.Parameters.PercentageElite,
                ProbabilityMutation = algorithm.Parameters.ProbabilityMutation,
                CrossoverType = algorithm.Parameters.CrossoverType,
                MutationType = algorithm.Parameters.MutationType
            };
            // Return the page.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the provided model isn't valid.
            if (!ModelState.IsValid)
            {
                // Add an error to the model.
                ModelState.AddModelError(string.Empty, "An error has been encountered. Please check again the input fields.");
                // Redisplay the page.
                return Page();
            }
            // Get the edges.
            var edges = JsonSerializer.Deserialize<IEnumerable<Edge>>(Input.Edges)
                .Where(item => !string.IsNullOrEmpty(item.SourceNode) && !string.IsNullOrEmpty(item.TargetNode))
                .Distinct();
            // Check if there haven't been any edges found.
            if (edges == null || !edges.Any())
            {
                // Add an error to the model.
                ModelState.AddModelError(string.Empty, "None of the provided edges contains both source and target nodes.");
                // Redisplay the page.
                return Page();
            }
            // Get a list of all the nodes in the edges.
            var nodes = edges
                .Select(item => item.SourceNode)
                .Concat(edges.Select(item => item.TargetNode))
                .Distinct();
            // Get the target nodes.
            var targetNodes = JsonSerializer.Deserialize<IEnumerable<string>>(Input.TargetNodes).Intersect(nodes);
            // Get the preferred nodes.
            var preferredNodes = JsonSerializer.Deserialize<IEnumerable<string>>(Input.PreferredNodes).Intersect(nodes);
            // Check if there haven't been any target nodes found.
            if (!targetNodes.Any())
            {
                // Add an error to the model.
                ModelState.AddModelError(string.Empty, "No target nodes could be found with the given data.");
                // Redisplay the page.
                return Page();
            }
            // Define the parameters.
            var parameters = new Parameters
            {
                RandomSeed = Input.RandomSeed,
                MaximumIterations = Input.MaximumIterations,
                MaximumIterationsWithoutImprovement = Input.MaximumIterationsWithoutImprovement,
                MaximumPathLength = Input.MaximumPathLength,
                PopulationSize = Input.PopulationSize,
                RandomGenesPerChromosome = Input.RandomGenesPerChromosome,
                PercentageElite = Input.PercentageElite,
                PercentageRandom = Input.PercentageRandom,
                ProbabilityMutation = Input.ProbabilityMutation,
                CrossoverType = Input.CrossoverType,
                MutationType = Input.MutationType
            };
            // Check if all of the parameters are valid.
            if (!parameters.IsValid())
            {
                // Add an error to the model.
                ModelState.AddModelError(string.Empty, "One or more of the given parameters are invalid. Please check again the corresponding fields.");
                // Redisplay the page.
                return Page();
            }
            // Define the new algorithm instance.
            var algorithm = new Algorithm
            {
                Id = Guid.NewGuid().ToString(),
                Name = Input.Name,
                DateTimeStarted = null,
                DateTimeEnded = null,
                DateTimePeriods = new List<DateTimePeriod>(),
                Status = AlgorithmStatus.Scheduled,
                Nodes = nodes.ToList(),
                Edges = edges.ToList(),
                TargetNodes = targetNodes.ToList(),
                PreferredNodes = preferredNodes.ToList(),
                CurrentIteration = 0,
                CurrentIterationWithoutImprovement = 0,
                Parameters = parameters,
                Population = new Population()
            };
            // Mark it for addition.
            _context.Algorithms.Add(algorithm);
            // Try to save the changes in the database.
            try
            {
                // Save the changes in the database.
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                // If we made it this far, something went wrong with adding to the database, so we add an error to the model.
                ModelState.AddModelError(string.Empty, exception.Message);
                // And re-display the page.
                return Page();
            }
            // Display a message.
            TempData["StatusMessage"] = "Success: 1 algorithm created successfully and scheduled.";
            // Redirect to the index page.
            return RedirectToPage("/Dashboard");
        }
    }
}
