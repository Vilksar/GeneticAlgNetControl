using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Helpers.Extensions;
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
            public int RandomSeed { get; set; } = DefaultValues.RandomSeed;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumIterations { get; set; } = DefaultValues.MaximumIterations;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumIterationsWithoutImprovement { get; set; } = DefaultValues.MaximumIterationsWithoutImprovement;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int MaximumPathLength { get; set; } = DefaultValues.MaximumPathLength;

            [Required(ErrorMessage = "This field is required.")]
            [Range(2, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int PopulationSize { get; set; } = DefaultValues.PopulationSize;

            [Required(ErrorMessage = "This field is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "The value must be between {1} and {2}.")]
            public int RandomGenesPerChromosome { get; set; } = DefaultValues.RandomGenesPerChromosome;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double PercentageRandom { get; set; } = DefaultValues.PercentageRandom;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double PercentageElite { get; set; } = DefaultValues.PercentageElite;

            [Required(ErrorMessage = "This field is required.")]
            [Range(0.00, 1.00, ErrorMessage = "The value must be between {1} and {2}.")]
            public double ProbabilityMutation { get; set; } = DefaultValues.ProbabilityMutation;

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

            public static class DefaultValues
            {
                public static int RandomSeed { get; } = (new Random()).Next();

                public static int MaximumIterations { get; } = 10000;

                public static int MaximumIterationsWithoutImprovement { get; } = 1000;

                public static int MaximumPathLength { get; } = 5;

                public static int PopulationSize { get; } = 80;

                public static int RandomGenesPerChromosome { get; } = 25;

                public static double PercentageRandom { get; } = 0.25;

                public static double PercentageElite { get; } = 0.25;

                public static double ProbabilityMutation { get; } = 0.01;
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
            var algorithmRun = _context.AlgorithmRuns
                .Include(item => item.AlgorithmData)
                .Include(item => item.AlgorithmParameters)
                .FirstOrDefault(item => item.Id == id);
            // Check if there was no item found.
            if (algorithmRun == null)
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: There is no custom analysis with the specified ID or you do not have access to it.";
                // Redirect to the overview page.
                return RedirectToPage("/Overview");
            }
            // Define the input.
            Input = new InputModel
            {
                Name = algorithmRun.Name,
                Edges = JsonSerializer.Serialize(algorithmRun.AlgorithmData.Edges),
                TargetNodes = JsonSerializer.Serialize(algorithmRun.AlgorithmData.TargetNodes),
                PreferredNodes = JsonSerializer.Serialize(algorithmRun.AlgorithmData.PreferredNodes),
                RandomSeed = algorithmRun.AlgorithmParameters.RandomSeed,
                MaximumIterations = algorithmRun.AlgorithmParameters.MaximumIterations,
                MaximumIterationsWithoutImprovement = algorithmRun.AlgorithmParameters.MaximumIterationsWithoutImprovement,
                MaximumPathLength = algorithmRun.AlgorithmParameters.MaximumPathLength,
                PopulationSize = algorithmRun.AlgorithmParameters.PopulationSize,
                RandomGenesPerChromosome = algorithmRun.AlgorithmParameters.RandomGenesPerChromosome,
                PercentageRandom = algorithmRun.AlgorithmParameters.PercentageRandom,
                PercentageElite = algorithmRun.AlgorithmParameters.PercentageElite,
                ProbabilityMutation = algorithmRun.AlgorithmParameters.ProbabilityMutation
            };
            // Return the page.
            return Page();
        }
    }
}
