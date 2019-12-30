using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static GeneticAlgNetControl.Data.Models.Analysis;

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
            public string Id { get; set; }

            public string Name { get; set; }

            public AnalysisStatus Status { get; set; }

            public DateTime? DateTimeStarted { get; set; }

            public DateTime? DateTimeEnded { get; set; }

            public TimeSpan DateTimeSpan { get; set; }

            public int CurrentIteration { get; set; }

            public int CurrentIterationWithoutImprovement { get; set; }

            public List<string> Nodes { get; set; }

            public List<Edge> Edges { get; set; }

            public List<string> TargetNodes { get; set; }

            public List<string> PreferredNodes { get; set; }

            public Parameters Parameters { get; set; }

            public Population Population { get; set; }

            public List<double> HistoricBestFitness { get; set; }

            public List<double> HistoricAverageFitness { get; set; }

            public List<Solution> Solutions { get; set; }
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
            var analysis = _context.Analyses
                    .FirstOrDefault(item => item.Id == id);
            // Check if the item has not been found.
            if (analysis == null)
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No item has been found with the provided ID.";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Define the view.
            View = new ViewModel
            {
                Id = analysis.Id,
                Name = analysis.Name,
                Status = analysis.Status,
                DateTimeStarted = analysis.DateTimeStarted,
                DateTimeEnded = analysis.DateTimeEnded,
                DateTimeSpan = JsonSerializer.Deserialize<List<DateTimeInterval>>(analysis.DateTimeIntervals).Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value),
                CurrentIteration = analysis.CurrentIteration,
                CurrentIterationWithoutImprovement = analysis.CurrentIterationWithoutImprovement,
                Nodes = JsonSerializer.Deserialize<List<string>>(analysis.Nodes),
                Edges = JsonSerializer.Deserialize<List<Edge>>(analysis.Edges),
                TargetNodes = JsonSerializer.Deserialize<List<string>>(analysis.TargetNodes),
                PreferredNodes = JsonSerializer.Deserialize<List<string>>(analysis.PreferredNodes),
                Parameters = JsonSerializer.Deserialize<Parameters>(analysis.Parameters),
                Population = JsonSerializer.Deserialize<Population>(analysis.Population),
                HistoricBestFitness = JsonSerializer.Deserialize<List<double>>(analysis.HistoricBestFitness),
                HistoricAverageFitness = JsonSerializer.Deserialize<List<double>>(analysis.HistoricAverageFitness),
                Solutions = JsonSerializer.Deserialize<List<Solution>>(analysis.Solutions)
            };
            // Return the page.
            return Page();
        }

        public IActionResult OnGetRefresh(string id)
        {
            // Get the analysis with the provided ID.
            var analysis = _context.Analyses.FirstOrDefault(item => item.Id == id);
            // Check if there wasn't any analysis found.
            if (analysis == null)
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
            // Get the parameters.
            var parameters = JsonSerializer.Deserialize<Parameters>(analysis.Parameters);
            // Get the required values.
            var status = analysis.Status;
            var currentIteration = analysis.CurrentIteration;
            var currentIterationWithoutImprovement = analysis.CurrentIterationWithoutImprovement;
            var maximumIterations = parameters.MaximumIterations;
            var maximumIterationsWithoutImprovement = parameters.MaximumIterationsWithoutImprovement;
            var dateTimeStarted = analysis.DateTimeStarted;
            var timeSpan = JsonSerializer.Deserialize<List<DateTimeInterval>>(analysis.DateTimeIntervals).Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value);
            var dateTimeEnded = analysis.DateTimeEnded;
            // Return a new JSON object.
            return new JsonResult(new
            {
                StatusTitle = status.ToString(),
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
