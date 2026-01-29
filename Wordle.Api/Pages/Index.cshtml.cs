using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wordle.Api.Pages
{
    public class IndexModel : PageModel
    {
        public string Title { get; set; } = "Welcome to Wordle API";
        public int Counter { get; set; } = 5;
        public string Name { get; set; } = "Wordle User";
        private static int InternalCounter = 0;
        public void OnGet()
        {
            Counter = InternalCounter;
        }

        public IActionResult OnPost()
        {
            InternalCounter++;
            Counter = InternalCounter;
            return Page();
        }
    }
}
