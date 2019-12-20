using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class StopModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StopModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : IValidatableObject
        {
            [Required(ErrorMessage = "This field is required.")]
            public string Ids { get; set; }

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
                    .Where(item => id.Contains(item.Id) && item.Status == AnalysisStatus.Ongoing)
            };
            // Check if there weren't any items found.
            if (View.Items == null || !View.Items.Any())
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No items have been found with the provided IDs or none of the items have the required status of \"Ongoing\".";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Get the IDs of the items found.
            Input = new InputModel
            {
                Ids = JsonSerializer.Serialize(View.Items.Select(item => item.Id))
            };
            // Return the page.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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
                    .Where(item => itemIds.Contains(item.Id) && item.Status == AnalysisStatus.Ongoing)
            };
            // Check if there weren't any items found.
            if (View.Items == null || !View.Items.Any())
            {
                // Display a message.
                TempData["StatusMessage"] = "Error: No items have been found with the provided IDs or none of the items have the required status of \"Ongoing\".";
                // Redirect to the index page.
                return RedirectToPage("/Dashboard");
            }
            // Save the number of items found.
            var itemCount = View.Items.Count();
            // Mark the items for updating.
            _context.UpdateRange(View.Items);
            // Go over each of the items.
            foreach (var item in View.Items)
            {
                // Update its status.
                item.Status = AnalysisStatus.Stopping;
            }
            // Try to update the database.
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
            TempData["StatusMessage"] = $"Success: {itemCount.ToString()} item{(itemCount != 1 ? "s" : string.Empty)} have been successfully scheduled to stop.";
            // Redirect to the index page.
            return RedirectToPage("/Dashboard");
        }
    }
}
