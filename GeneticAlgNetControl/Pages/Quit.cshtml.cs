using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeneticAlgNetControl.Pages
{
    public class QuitModel : PageModel
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<QuitModel> _logger;

        public QuitModel(IHostApplicationLifetime hostApplicationLifetime, ILogger<QuitModel> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Return the page.
            return Page();
        }

        public IActionResult OnPost()
        {
            // Log a message.
            _logger.LogInformation("Application is shutting down...");
            // Stop the application.
            _hostApplicationLifetime.StopApplication();
            // Return the page.
            return new NoContentResult();
        }
    }
}
