using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
                    Status = string.Empty,
                    CurrentIteration = string.Empty,
                    CurrentIterationWithoutImprovement = string.Empty
                });
            }
            // Return a new JSON object.
            return new JsonResult(new
            {
                Status = algorithm.Status.ToString(),
                CurrentIteration = algorithm.CurrentIteration,
                CurrentIterationWithoutImprovement = algorithm.CurrentIterationWithoutImprovement
            });
        }
    }
}
