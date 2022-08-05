using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerCarePortal.Pages.Tickets
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class EditAdminModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditAdminModel(ApplicationDbContext context)
        {
            _db = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;

        //For comment section
        public List<Comment> Comments { get; set; } = default!;
        [BindProperty]
        public Comment comment { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _db.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _db.Tickets
                .Include(d => d.TeamAssigned)
                .Include(d => d.History)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            Comments = await _db.Comments
                        .Where(c => c.HistoryId == ticket.History.Id)
                        .OrderByDescending(c => c.Id)
                        .ToListAsync();
            Ticket = ticket;
            ViewData["Teams"] = new SelectList(_db.Teams, "Id", "Name");
            return Page();
        }
        //OnPost Handler

        public async Task<IActionResult> OnPostAdminHandler()
        {
            if (Ticket is null)
            {
                return Page();
            }

            try
            {
                Ticket.TeamAssigned = _db.Teams.FirstOrDefault(t=>t.Id == Ticket.TeamAssigned.Id);
                _db.Tickets.Update(Ticket);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(Ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }


        private bool TicketExists(int id)
        {
            return (_db.Agents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> OnPostAddComment()
        {
            try
            {
                if (comment.Description.Trim() != "")
                {
                    string description = DateTime.Now.ToLocalTime().ToString() + ": " + User.Identity.Name.ToString() + ": " + comment.Description;
                    comment.Description = description;
                    comment.HistoryId = Ticket.History.Id;
                    await _db.Comments.AddAsync(comment);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return RedirectToPage("./EditAdmin", new { id = Ticket.Id });
        }
    }
}