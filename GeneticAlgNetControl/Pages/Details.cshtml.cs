using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ViewModel View { get; set; }

        public class ViewModel
        {
            public Algorithm Algorithm { get; set; }
        }

        public IActionResult OnGet(string id)
        {
            // Check if there isn't any ID provided.
            if (string.IsNullOrEmpty(id))
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No ID has been provided.";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Get the item.
            var algorithm = _context.Algorithms
                    .FirstOrDefault(item => item.Id == id);
            // Check if the item has not been found.
            if (algorithm == null)
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No item has been found with the provided ID.";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Define the view.
            View = new ViewModel
            {
                Algorithm = algorithm
            };
            // Return the page.
            return Page();
        }

        public IActionResult OnGetRefresh(string id)
        {
            // Get the algorithm with the provided ID.
            var algorithm = _context.Algorithms.FirstOrDefault(item => item.Id == id);
            // Check if there wasn't any algorithm found.
            if (algorithm == null)
            {
                // Return an empty JSON object.
                return new JsonResult(new
                {
                    StatusTitle = string.Empty,
                    StatusText = string.Empty,
                    CurrentIterationTitle = string.Empty,
                    CurrentIterationText = string.Empty,
                    CurrentIterationWithoutImprovementTitle = string.Empty,
                    CurrentIterationWithoutImprovementText = string.Empty,
                    TimeSpanTitle = string.Empty,
                    TimeSpanText = string.Empty
                });
            }
            // Get the required values.
            var status = algorithm.Status;
            var currentIteration = algorithm.CurrentIteration;
            var currentIterationWithoutImprovement = algorithm.CurrentIterationWithoutImprovement;
            var maximumIterations = algorithm.Parameters.MaximumIterations;
            var maximumIterationsWithoutImprovement = algorithm.Parameters.MaximumIterationsWithoutImprovement;
            var dateTimeStarted = algorithm.DateTimeStarted;
            var timeSpan = algorithm.DateTimePeriods.Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value);
            var dateTimeEnded = algorithm.DateTimeEnded;
            // Return a new JSON object.
            return new JsonResult(new
            {
                StatusTitle = status.ToActualString(),
                StatusText = status.ToString(),
                CurrentIterationTitle = $"{currentIteration} / {maximumIterations}",
                CurrentIterationText = $"{currentIteration} / {maximumIterations}",
                CurrentIterationWithoutImprovementTitle = $"{currentIterationWithoutImprovement} / {maximumIterationsWithoutImprovement}",
                CurrentIterationWithoutImprovementText = $"{currentIterationWithoutImprovement} / {maximumIterationsWithoutImprovement}",
                DateTimeStartedTitle = dateTimeStarted != null ? dateTimeStarted.Value.ToString() : "Not started yet.",
                DateTimeStartedText = dateTimeStarted != null ? dateTimeStarted.Value.ToLongTimeString() : "--:--:-- --",
                TimeSpanTitle = timeSpan.ToString(),
                TimeSpanText = timeSpan.ToString("dd\\:hh\\:mm\\:ss"),
                DateTimeEndedTitle = dateTimeEnded != null ? dateTimeEnded.Value.ToString() : "Not ended yet.",
                DateTimeEndedText = dateTimeEnded != null ? dateTimeEnded.Value.ToLongTimeString() : "--:--:-- --"
            });
        }
    }
}
