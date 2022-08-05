using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CustomerCarePortal.Pages.Tracking
{
    public class IndexModel : PageModel
    {
        public ApplicationDbContext _db;
        [BindProperty]
        public Ticket Ticket { get; set; } = default!; [BindProperty]
        public Comment comment { get; set; } = default!;
        public List<Comment> Comments { get; set; } = default!;
        public Boolean Flag { get; set; } = true;
        public string ID { get; set; } = "";
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id is not null)
            {
                ID = id;
                var ticket = _db.Tickets
                    .Include(t => t.History)
                    .FirstOrDefault(t => t.TrackingId.Equals(id));
                if (ticket is not null)
                {
                    Flag = false;
                    ticket.CurrentState = await _db.States.FindAsync(ticket.CurrentStateId);
                    Comments = await _db.Comments
                        .Where(c => c.HistoryId == ticket.History.Id)
                        .OrderByDescending(c=>c.Id)
                        .ToListAsync();
                    Ticket = ticket;
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostTrack()
        {
            if (Ticket is not null)
            {
                return RedirectToPage("/Tracking/Index", new { id = Ticket.TrackingId });
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAddComment()
        {
            try
            {
                if (comment.Description != null && comment.Description.Trim() != "")
                {
                    string description = DateTime.Now.ToLocalTime().ToString() + ": " + Ticket.Email + ": " + comment.Description;
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
            return RedirectToPage("/Tracking/Index", new { id = Ticket.TrackingId });
        }
    }
}
