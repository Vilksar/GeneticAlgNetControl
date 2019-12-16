using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeneticAlgNetControl.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Return the page.
            return Page();
        }
    }
}
