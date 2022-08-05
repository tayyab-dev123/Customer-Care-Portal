using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerCarePortal.Pages.Tickets
{
    public class SuccessModel : PageModel
    {
        public ApplicationDbContext _db;
        [BindProperty]
        public Ticket Ticket { get; set; }
        public SuccessModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket is null)
            {
                return NotFound();
            }
            Ticket = ticket;
            return Page();
        }
    }
}
