using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Extensions;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class SaveModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SaveModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : IValidatableObject
        {
            [Required(ErrorMessage = "This field is required.")]
            public string Ids { get; set; }

            [Required(ErrorMessage = "This field is required.")]
            public string Type { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                // Check if the IDs string is a valid JSON object.
                if (Ids.IsNullOrInvalidJsonObject<IEnumerable<string>>())
                {
                    yield return new ValidationResult("The value is not a valid JSON string.", new List<string> { nameof(Ids) });
                }
            }
        }

        public ViewModel View { get; set; }

        public class ViewModel
        {
            public IEnumerable<Analysis> Items { get; set; }
        }

        public IActionResult OnGet(IEnumerable<string> id)
        {
            // Check if there aren't any (valid) IDs provided.
            if (id == null || !id.Any())
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No or invalid IDs have been provided.";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Define the view.
            View = new ViewModel
            {
                Items = _context.Analyses
                    .Where(item => id.Contains(item.Id) && (item.Status == AnalysisStatus.Stopped || item.Status == AnalysisStatus.Completed))
            };
            // Check if there weren't any items found.
            if (View.Items == null || !View.Items.Any())
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No items have been found with the provided IDs or none of the items have the required status of \"Stopped\", or \"Completed\".";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Get the IDs of the items found.
            Input = new InputModel
            {
                Ids = JsonSerializer.Serialize(View.Items.Select(item => item.Id)),
                Type = "Json"
            };
            // Return the page.
            return Page();
        }

        public IActionResult OnPost()
        {
            // Check if the provided model is not valid.
            if (!ModelState.IsValid)
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No or invalid IDs have been provided.";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Get all of the individual IDs to look for.
            var itemIds = JsonSerializer.Deserialize<IEnumerable<string>>(Input.Ids);
            // Define the view.
            View = new ViewModel
            {
                Items = _context.Analyses
                    .Where(item => itemIds.Contains(item.Id) && (item.Status == AnalysisStatus.Stopped || item.Status == AnalysisStatus.Completed))
            };
            // Check if there weren't any items found.
            if (View.Items == null || !View.Items.Any())
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No items have been found with the provided IDs or none of the items have the required status of \"Stopped\", or \"Completed\".";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Save the number of items found.
            var itemCount = View.Items.Count();
            // Define the stream of the file to return.
            var zipStream = new MemoryStream();
            // Define a new ZIP archive.
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                // Go over each of the items.
                foreach (var item in View.Items)
                {
                    // Get the analysis related data.
                    var dateTimePeriods = JsonSerializer.Deserialize<List<DateTimePeriod>>(item.DateTimePeriods);
                    var parameters = JsonSerializer.Deserialize<Parameters>(item.Parameters);
                    var population = JsonSerializer.Deserialize<Population>(item.Population);
                    // Define a new memory stream.
                    var memoryStream = new MemoryStream();
                    // Check the file type to return.
                    if (Input.Type == "Json")
                    {
                        // Define the text to be written to the file.
                        var outputText = JsonSerializer.Serialize(new
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Status = item.Status.ToString(),
                            CurrentIteration = item.CurrentIteration,
                            CurrentIterationWithoutImprovement = item.CurrentIterationWithoutImprovement,
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
                        // Define the stream of the file to be downloaded.
                        memoryStream.Write(Encoding.UTF8.GetBytes(outputText));
                    }
                    else
                    {
                        // Add an error to the model.
                        ModelState.AddModelError(string.Empty, "The provided file type is not valid or not yet implemented.");
                        // Redisplay the page.
                        return Page();
                    }
                    // Create a new entry in the archive and open it for writing.
                    using (var stream = archive.CreateEntry($"{item.Name}_{item.Id}.{Input.Type.ToLower()}", CompressionLevel.Fastest).Open())
                    {
                        // Write the memory stream to the new archive entry.
                        stream.Write(memoryStream.ToArray());
                    }
                }
            }
            // Return the file.
            return File(zipStream.ToArray(), "application/zip", $"Analyses-{DateTime.Now.ToShortDateString()}.zip");
        }
    }
}
