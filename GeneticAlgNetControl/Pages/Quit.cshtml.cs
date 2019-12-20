using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace GeneticAlgNetControl.Pages
{
    public class QuitModel : PageModel
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public QuitModel(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public IActionResult OnGet()
        {
            // Return the page.
            return Page();
        }

        public IActionResult OnPost()
        {
            // Stop the application.
            _hostApplicationLifetime.StopApplication();
            // Return the page.
            return new NoContentResult();
        }
    }
}
