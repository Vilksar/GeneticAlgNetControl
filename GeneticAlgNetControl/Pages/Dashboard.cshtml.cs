using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly LinkGenerator _linkGenerator;

        public DashboardModel(ILogger<IndexModel> logger, ApplicationDbContext context, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _context = context;
            _linkGenerator = linkGenerator;
        }

        public ViewModel View { get; set; }

        public class ViewModel
        {
            public string SearchString { get; set; }

            public IEnumerable<string> SearchIn { get; set; }

            public IEnumerable<string> Filter { get; set; }

            public string SortBy { get; set; }

            public string SortDirection { get; set; }

            public int ItemsPerPage { get; set; }

            public int CurrentPage { get; set; }

            public int TotalItems { get; set; }

            public int TotalPages { get; set; }

            public int ItemsPerPageFirst { get; set; }

            public int ItemsPerPageLast { get; set; }

            public string PreviousPageUrl { get; set; }

            public string NextPageUrl { get; set; }

            public Dictionary<string, string> AppliedFilters { get; set; }

            public IEnumerable<Algorithm> Items { get; set; }
        }

        public IActionResult OnGet(string searchString, IEnumerable<string> searchIn, IEnumerable<string> filter, string sortBy, string sortDirection, int itemsPerPage, int currentPage)
        {
            // Check if any of the parameters is missing or invalid.
            if (searchIn == null || filter == null || string.IsNullOrEmpty(sortBy) || string.IsNullOrEmpty(sortDirection) || itemsPerPage <= 0 || currentPage <= 0)
            {
                // Assign the default values to the missing or invalid parameters.
                searchIn = searchIn != null ? searchIn : Enumerable.Empty<string>();
                filter = filter != null ? filter : Enumerable.Empty<string>();
                sortBy = !string.IsNullOrEmpty(sortBy) ? sortBy : "DateTimeStarted";
                sortDirection = !string.IsNullOrEmpty(sortDirection) ? sortBy : "Descending";
                itemsPerPage = 0 < itemsPerPage ? itemsPerPage : 5;
                currentPage = 0 < currentPage ? currentPage : 1;
                // Redirect to the current page, using the default values.
                return RedirectToPage(new { searchString = searchString, searchIn = searchIn, filter = filter, sortBy = sortBy, sortDirection = sortDirection, itemsPerPage = itemsPerPage, currentPage = currentPage });
            }
            // Save all parameters to the view.
            View = new ViewModel
            {
                SearchString = !string.IsNullOrEmpty(searchString) ? searchString : string.Empty,
                SearchIn = searchIn,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection,
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage
            };
            View.SearchString = searchString;
            View.SearchIn = searchIn;
            View.Filter = filter;
            View.SortBy = sortBy;
            View.SortDirection = sortDirection;
            View.ItemsPerPage = itemsPerPage;
            View.CurrentPage = currentPage;
            // Start with all of the items in the database that match the search string.
            var query = _context.Algorithms
                .Where(item => !View.SearchIn.Any() ||
                View.SearchIn.Contains("Id") && item.Id.Contains(View.SearchString) ||
                View.SearchIn.Contains("Name") && item.Name.Contains(View.SearchString) ||
                View.SearchIn.Contains("Nodes") && item.Edges.Any(item1 => item1.SourceNode.Contains(View.SearchString) || item1.TargetNode.Contains(View.SearchString)) ||
                View.SearchIn.Contains("TargetNodes") && item.TargetNodes.Any(item1 => item1.Contains(View.SearchString)) ||
                View.SearchIn.Contains("PreferredNodes") && item.PreferredNodes.Any(item1 => item1.Contains(View.SearchString)));
            // Select the results matching the filter parameter.
            query = query
                .Where(item => View.Filter.Contains("IsScheduled") ? item.Status == AlgorithmStatus.Scheduled : true)
                .Where(item => View.Filter.Contains("IsOngoing") ? item.Status == AlgorithmStatus.Ongoing : true)
                .Where(item => View.Filter.Contains("IsScheduledToStop") ? item.Status == AlgorithmStatus.ScheduledToStop : true)
                .Where(item => View.Filter.Contains("IsStopped") ? item.Status == AlgorithmStatus.Stopped : true)
                .Where(item => View.Filter.Contains("IsCompleted") ? item.Status == AlgorithmStatus.Completed : true);
            // Sort it according to the parameters.
            switch ((View.SortBy, View.SortDirection))
            {
                case var sort when sort == ("Id", "Ascending"):
                    query = query.OrderBy(item => item.Id);
                    break;
                case var sort when sort == ("Id", "Descending"):
                    query = query.OrderByDescending(item => item.Id);
                    break;
                case var sort when sort == ("DateTimeStarted", "Ascending"):
                    query = query.OrderBy(item => item.DateTimeStarted);
                    break;
                case var sort when sort == ("DateTimeStarted", "Descending"):
                    query = query.OrderByDescending(item => item.DateTimeStarted);
                    break;
                case var sort when sort == ("DateTimeEnded", "Ascending"):
                    query = query.OrderBy(item => item.DateTimeEnded);
                    break;
                case var sort when sort == ("DateTimeEnded", "Descending"):
                    query = query.OrderByDescending(item => item.DateTimeEnded);
                    break;
                case var sort when sort == ("Name", "Ascending"):
                    query = query.OrderBy(item => item.Name);
                    break;
                case var sort when sort == ("Name", "Descending"):
                    query = query.OrderByDescending(item => item.Name);
                    break;
                case var sort when sort == ("Status", "Ascending"):
                    query = query.OrderBy(item => item.Status);
                    break;
                case var sort when sort == ("Status", "Descending"):
                    query = query.OrderByDescending(item => item.Status);
                    break;
                case var sort when sort == ("NodeCount", "Ascending"):
                    query = query.OrderBy(item => item.Nodes.Count());
                    break;
                case var sort when sort == ("NodeCount", "Descending"):
                    query = query.OrderByDescending(item => item.Nodes.Count());
                    break;
                case var sort when sort == ("TargetNodeCount", "Ascending"):
                    query = query.OrderBy(item => item.TargetNodes.Count());
                    break;
                case var sort when sort == ("TargetNodeCount", "Descending"):
                    query = query.OrderByDescending(item => item.TargetNodes.Count());
                    break;
                case var sort when sort == ("PreferredNodeCount", "Ascending"):
                    query = query.OrderBy(item => item.PreferredNodes.Count());
                    break;
                case var sort when sort == ("PreferredNodeCount", "Descending"):
                    query = query.OrderByDescending(item => item.PreferredNodes.Count());
                    break;
                case var sort when sort == ("EdgeCount", "Ascending"):
                    query = query.OrderBy(item => item.Edges.Count());
                    break;
                case var sort when sort == ("EdgeCount", "Descending"):
                    query = query.OrderByDescending(item => item.Edges.Count());
                    break;
                default:
                    break;
            }
            // Get the pagination variables.
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / View.ItemsPerPage);
            var itemsPerPageFirst = 0 < totalItems ? (View.CurrentPage - 1) * View.ItemsPerPage + 1 : 0;
            var itemsPerPageLast = 0 < totalItems ? View.CurrentPage * Math.Min(View.ItemsPerPage, totalItems) < totalItems ? View.CurrentPage * Math.Min(View.ItemsPerPage, totalItems) : totalItems : 0;
            // Define the rest of the parameters of the view.
            View.TotalItems = totalItems;
            View.TotalPages = totalPages;
            View.ItemsPerPageFirst = itemsPerPageFirst;
            View.ItemsPerPageLast = itemsPerPageLast;
            View.PreviousPageUrl = View.CurrentPage == 1 || totalPages == 0 ? null : _linkGenerator.GetPathByRouteValues(httpContext: HttpContext, routeName: null, values: new { searchString = View.SearchString, searchIn = View.SearchIn, filter = View.Filter, sortBy = View.SortBy, sortDirection = View.SortDirection, itemsPerPage = View.ItemsPerPage, currentPage = View.CurrentPage - 1 });
            View.NextPageUrl = View.CurrentPage == totalPages || totalPages == 0 ? null : _linkGenerator.GetPathByRouteValues(httpContext: HttpContext, routeName: null, values: new { searchString = View.SearchString, searchIn = View.SearchIn, filter = View.Filter, sortBy = View.SortBy, sortDirection = View.SortDirection, itemsPerPage = View.ItemsPerPage, currentPage = View.CurrentPage + 1 });
            View.AppliedFilters = new Dictionary<string, string>();
            // Get the items to return to the view.
            View.Items = query
                .Skip((View.CurrentPage - 1) * View.ItemsPerPage)
                .Take(View.ItemsPerPage)
                .AsEnumerable();
            // Return the page.
            return Page();
        }

        public IActionResult OnGetRefresh(string id = null)
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
                    TimeSpan = string.Empty,
                    ProgressIterations = string.Empty,
                    ProgressIterationsWithoutImprovement = string.Empty
                });
            }
            // Get the required data.
            var status = algorithm.Status;
            var timeSpan = algorithm.DateTimePeriods.Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value);
            var progressIterations = (double)algorithm.CurrentIteration / algorithm.Parameters.MaximumIterations * 100;
            var progressIterationsWithoutImprovement = (double)algorithm.CurrentIterationWithoutImprovement / algorithm.Parameters.MaximumIterationsWithoutImprovement * 100;
            // Return a new JSON object.
            return new JsonResult(new
            {
                StatusTitle = status.ToString(),
                StatusText = status.ToString(),
                TimeSpanTitle = timeSpan.ToString(),
                TimeSpanText = timeSpan.ToString("dd\\:hh\\:mm\\:ss"),
                ProgressIterationsTitle = progressIterations.ToString(),
                ProgressIterationsText = progressIterations.ToString("0.00") + "%",
                ProgressIterationsWithoutImprovementTitle = progressIterationsWithoutImprovement.ToString(),
                ProgressIterationsWithoutImprovementText = progressIterationsWithoutImprovement.ToString("0.00") + "%"
            });
        }
    }
}
