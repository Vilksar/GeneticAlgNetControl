using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class OverviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly LinkGenerator _linkGenerator;

        public OverviewModel(ILogger<IndexModel> logger, ApplicationDbContext context, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _context = context;
            _linkGenerator = linkGenerator;
        }

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; } = string.Empty;

            public IEnumerable<string> SearchIn { get; set; } = Enumerable.Empty<string>();

            public IEnumerable<string> Filter { get; set; } = Enumerable.Empty<string>();

            public string SortBy { get; set; } = "DateTimeStarted";

            public string SortDirection { get; set; } = "Descending";

            public int ItemsPerPage { get; set; } = 5;

            public int CurrentPage { get; set; } = 1;
        }

        public ViewModel View { get; set; }

        public class ViewModel
        {
            public int TotalItems { get; set; }

            public int CurrentPage { get; set; }

            public int TotalPages { get; set; }

            public int ItemsPerPageFirst { get; set; }

            public int ItemsPerPageLast { get; set; }

            public string PreviousPageUrl { get; set; }

            public string NextPageUrl { get; set; }

            public Dictionary<string, string> AppliedFilters { get; set; }

            public IEnumerable<AlgorithmOverviewViewModel> Items { get; set; }
        }

        public IActionResult OnGet(InputModel input = null)
        {
            // Define the input
            Input = input ?? new InputModel();
            // Start with all of the items in the database that match the search string.
            var query = _context.Algorithms
                .Where(item => !Input.SearchIn.Any() ||
                Input.SearchIn.Contains("Id") && item.Id.Contains(Input.SearchString) ||
                Input.SearchIn.Contains("Name") && item.Name.Contains(Input.SearchString) ||
                Input.SearchIn.Contains("Nodes") && item.Edges.Any(item1 => item1.SourceNode.Contains(Input.SearchString) || item1.TargetNode.Contains(Input.SearchString)) ||
                Input.SearchIn.Contains("TargetNodes") && item.TargetNodes.Any(item1 => item1.Contains(Input.SearchString)) ||
                Input.SearchIn.Contains("PreferredNodes") && item.PreferredNodes.Any(item1 => item1.Contains(Input.SearchString)));
            // Select the results matching the filter parameter.
            query = query
                .Where(item => Input.Filter.Contains("IsOngoing") ? item.Status == AlgorithmStatus.Ongoing : true)
                .Where(item => Input.Filter.Contains("IsPaused") ? item.Status == AlgorithmStatus.Paused : true)
                .Where(item => Input.Filter.Contains("IsStopped") ? item.Status == AlgorithmStatus.Stopped : true)
                .Where(item => Input.Filter.Contains("IsCompleted") ? item.Status == AlgorithmStatus.Completed : true);
            // Sort it according to the parameters.
            switch ((Input.SortBy, Input.SortDirection))
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
            var totalPages = (int)Math.Ceiling((double)totalItems / Input.ItemsPerPage);
            var itemsPerPageFirst = 0 < totalItems ? (Input.CurrentPage - 1) * Input.ItemsPerPage + 1 : 0;
            var itemsPerPageLast = 0 < totalItems ? Input.CurrentPage * Math.Min(Input.ItemsPerPage, totalItems) < totalItems ? Input.CurrentPage * Math.Min(Input.ItemsPerPage, totalItems) : totalItems : 0;
            // Define the view.
            View = new ViewModel
            {
                TotalItems = totalItems,
                CurrentPage = Input.CurrentPage,
                TotalPages = totalPages,
                ItemsPerPageFirst = itemsPerPageFirst,
                ItemsPerPageLast = itemsPerPageLast,
                PreviousPageUrl = Input.CurrentPage == 1 || totalPages == 0 ? null : _linkGenerator.GetPathByRouteValues(httpContext: HttpContext, routeName: null, values: new { searchString = Input.SearchString, searchIn = Input.SearchIn, filter = Input.Filter, sortBy = Input.SortBy, sortDirection = Input.SortDirection, itemsPerPage = Input.ItemsPerPage, currentPage = Input.CurrentPage - 1 }),
                NextPageUrl = Input.CurrentPage == totalPages || totalPages == 0 ? null : _linkGenerator.GetPathByRouteValues(httpContext: HttpContext, routeName: null, values: new { searchString = Input.SearchString, searchIn = Input.SearchIn, filter = Input.Filter, sortBy = Input.SortBy, sortDirection = Input.SortDirection, itemsPerPage = Input.ItemsPerPage, currentPage = Input.CurrentPage + 1 }),
                AppliedFilters = new Dictionary<string, string>()
            };
            // Get the items to return to the view.
            View.Items = query
                .Skip((Input.CurrentPage - 1) * Input.ItemsPerPage)
                .Take(Input.ItemsPerPage)
                .AsEnumerable()
                .Select(item => new AlgorithmOverviewViewModel(item));
            // Return the page.
            return Page();
        }
    }
}