using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public ViewModel View { get; set; }

        public class ViewModel
        {
            public string RequestId { get; set; }

            public string Message { get; set; }
        }

        public IActionResult OnGet()
        {
            // Get the current request ID.
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            // Get the features of the error.
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            // Get the error message.
            var message = $"{DateTime.Now.ToString()}: The request with the ID {requestId} encountered an error.";
            // Check if we had an exception.
            if (exceptionHandlerPathFeature != null)
            {
                // Append the path and the error to the message.
                message += $" The request tried to access the page {exceptionHandlerPathFeature?.Path} and returned the error {exceptionHandlerPathFeature?.Error?.Message}.";
            }
            // Check if we had a status code re-execution.
            if (statusCodeReExecuteFeature != null)
            {
                // Append the path and the error to the message.
                message += $" The request tried to access the page {statusCodeReExecuteFeature?.OriginalPath}.";
            }
            // Define the view.
            View = new ViewModel
            {
                RequestId = requestId,
                Message = message
            };
            // Log the information.
            _logger.LogError(View.Message);
            // Return the page.
            return Page();
        }
    }
}
