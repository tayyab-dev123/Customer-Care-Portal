using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CustomerCarePortal.Models;
using CustomerCarePortal.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerCarePortal.Pages.Tickets
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator,TeamManager,Agent")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _db.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole("Administrator"))
            {
                return RedirectToPage("./EditAdmin", new { id = ticket.Id });
            }
            else if (User.IsInRole("TeamManager"))
            {
                return RedirectToPage("./EditManager", new { id = ticket.Id });
            }
            if (User.IsInRole("Agent"))
            {
                return RedirectToPage("./EditAgent", new { id = ticket.Id });
            }

            return NotFound();
        }
    }
}
