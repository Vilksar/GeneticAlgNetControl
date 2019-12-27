using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Data.Enumerations;
using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static GeneticAlgNetControl.Data.Models.Analysis;

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

            public string ClearFiltersUrl { get; set; }

            public IEnumerable<Analysis> Items { get; set; }
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
                sortDirection = !string.IsNullOrEmpty(sortDirection) ? sortDirection : "Descending";
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
            // Start with all of the items in the database that match the search string.
            var query = _context.Analyses
                .Where(item => !View.SearchIn.Any() ||
                    View.SearchIn.Contains("Id") && item.Id.Contains(View.SearchString) ||
                    View.SearchIn.Contains("Name") && item.Name.Contains(View.SearchString) ||
                    View.SearchIn.Contains("Nodes") && item.Nodes.Contains(View.SearchString) ||
                    View.SearchIn.Contains("Edges") && item.Edges.Contains(View.SearchString) ||
                    View.SearchIn.Contains("TargetNodes") && item.TargetNodes.Contains(View.SearchString) ||
                    View.SearchIn.Contains("PreferredNodes") && item.PreferredNodes.Contains(View.SearchString));
            // Select the results matching the filter parameter.
            query = query
                .Where(item => View.Filter.Contains("IsScheduled") ? item.Status == AnalysisStatus.Scheduled : true)
                .Where(item => View.Filter.Contains("IsNotScheduled") ? item.Status != AnalysisStatus.Scheduled : true)
                .Where(item => View.Filter.Contains("IsInitializing") ? item.Status == AnalysisStatus.Initializing : true)
                .Where(item => View.Filter.Contains("IsNotInitializing") ? item.Status != AnalysisStatus.Initializing : true)
                .Where(item => View.Filter.Contains("IsOngoing") ? item.Status == AnalysisStatus.Ongoing : true)
                .Where(item => View.Filter.Contains("IsNotOngoing") ? item.Status != AnalysisStatus.Ongoing : true)
                .Where(item => View.Filter.Contains("IsStopping") ? item.Status == AnalysisStatus.Stopping : true)
                .Where(item => View.Filter.Contains("IsNotStopping") ? item.Status != AnalysisStatus.Stopping : true)
                .Where(item => View.Filter.Contains("IsStopped") ? item.Status == AnalysisStatus.Stopped : true)
                .Where(item => View.Filter.Contains("IsNotStopped") ? item.Status != AnalysisStatus.Stopped : true)
                .Where(item => View.Filter.Contains("IsCompleted") ? item.Status == AnalysisStatus.Completed : true)
                .Where(item => View.Filter.Contains("IsNotCompleted") ? item.Status != AnalysisStatus.Completed : true);
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
                case var sort when sort == ("NumberOfNodes", "Ascending"):
                    query = query.OrderBy(item => item.NumberOfNodes);
                    break;
                case var sort when sort == ("NumberOfNodes", "Descending"):
                    query = query.OrderByDescending(item => item.NumberOfNodes);
                    break;
                case var sort when sort == ("NumberOfEdges", "Ascending"):
                    query = query.OrderBy(item => item.NumberOfEdges);
                    break;
                case var sort when sort == ("NumberOfEdges", "Descending"):
                    query = query.OrderByDescending(item => item.NumberOfEdges);
                    break;
                case var sort when sort == ("NumberOfTargetNodes", "Ascending"):
                    query = query.OrderBy(item => item.NumberOfTargetNodes);
                    break;
                case var sort when sort == ("NumberOfTargetNodes", "Descending"):
                    query = query.OrderByDescending(item => item.NumberOfTargetNodes);
                    break;
                case var sort when sort == ("NumberOfPreferredNodes", "Ascending"):
                    query = query.OrderBy(item => item.NumberOfPreferredNodes);
                    break;
                case var sort when sort == ("NumberOfPreferredNodes", "Descending"):
                    query = query.OrderByDescending(item => item.NumberOfPreferredNodes);
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
            View.ClearFiltersUrl = string.IsNullOrEmpty(View.SearchString) && !View.Filter.Any() ? null : _linkGenerator.GetPathByRouteValues(httpContext: HttpContext, routeName: null, values: new { searchString = string.Empty, searchIn = Enumerable.Empty<string>(), filter = Enumerable.Empty<string>(), sortBy = View.SortBy, sortDirection = View.SortDirection, itemsPerPage = View.ItemsPerPage, currentPage = 1 });
            // Get the items to return to the view.
            View.Items = query
                .Skip((View.CurrentPage - 1) * View.ItemsPerPage)
                .Take(View.ItemsPerPage)
                .AsEnumerable();
            // Return the page.
            return Page();
        }

        public IActionResult OnGetRefreshStatistic(string statisticName)
        {
            // Define the count to return.
            var statisticCount = 0;
            // Get the required data, based on the statistic name.
            switch (statisticName)
            {
                case "Scheduled":
                    statisticCount = _context.Analyses.Count(item => item.Status == AnalysisStatus.Scheduled);
                    break;
                case "Ongoing":
                    statisticCount = _context.Analyses.Count(item => item.Status == AnalysisStatus.Initializing || item.Status == AnalysisStatus.Ongoing || item.Status == AnalysisStatus.Stopping);
                    break;
                case "Stopped":
                    statisticCount = _context.Analyses.Count(item => item.Status == AnalysisStatus.Stopped);
                    break;
                case "Completed":
                    statisticCount = _context.Analyses.Count(item => item.Status == AnalysisStatus.Completed);
                    break;
                default:
                    break;
            }
            // Return a new JSON object.
            return new JsonResult(new
            {
                StatisticName = statisticName,
                StatisticCount = statisticCount
            });
        }

        public IActionResult OnGetRefreshItem(string id)
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
                    TimeSpanTitle = string.Empty,
                    TimeSpanText = string.Empty,
                    ProgressIterationsTitle = string.Empty,
                    ProgressIterationsText = string.Empty,
                    ProgressIterationsWithoutImprovementTitle = string.Empty,
                    ProgressIterationsWithoutImprovementText = string.Empty
                });
            }
            // Get the parameters.
            var parameters = JsonSerializer.Deserialize<Parameters>(analysis.Parameters);
            // Get the required data.
            var status = analysis.Status;
            var timeSpan = JsonSerializer.Deserialize<List<DateTimeInterval>>(analysis.DateTimeIntervals).Select(item => (item.DateTimeEnded ?? DateTime.Now) - (item.DateTimeStarted ?? DateTime.Now)).Aggregate(TimeSpan.Zero, (sum, value) => sum + value);
            var progressIterations = (double)analysis.CurrentIteration / parameters.MaximumIterations * 100;
            var progressIterationsWithoutImprovement = (double)analysis.CurrentIterationWithoutImprovement / parameters.MaximumIterationsWithoutImprovement * 100;
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
