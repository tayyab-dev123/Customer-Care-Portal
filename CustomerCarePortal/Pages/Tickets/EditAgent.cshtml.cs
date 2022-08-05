using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CustomerCarePortal.Pages.Tickets
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Agent")]
    public class EditAgentModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditAgentModel(ApplicationDbContext context)
        {
            _db = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;
        //For Transitions
        [BindProperty]
        public Transition Transition { get; set; } = default!;

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
                .Include(d => d.AgentAssigned)
                .Include(d => d.Workflow)
                .Include(d => d.CurrentState)
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

            var Transitions = await _db.Transitions
                .Where(t => t.SourceStateId == Ticket.CurrentStateId)
                .Select(s => s)
                .ToListAsync();
            ViewData["Transitions"] = new SelectList(Transitions, "Id", "Name");
            return Page();
        }
        //OnPost Handler

        public async Task<IActionResult> OnPostAgentHandler()
        {
            if (Ticket is null)
            {
                return Page();
            }

            try
            {
                Ticket.TeamAssigned = _db.Teams.FirstOrDefault(t => t.Id == Ticket.TeamAssigned.Id);
                Ticket.AgentAssigned = _db.Agents.FirstOrDefault(a => a.Id == Ticket.AgentAssigned.Id);
                Ticket.Workflow = _db.Workflows.FirstOrDefault(w => w.Id == Ticket.WorkflowId);
                Transition = _db.Transitions.FirstOrDefault(t => t.Id == Transition.Id);
                if (Transition is not null)
                {
                    Ticket.CurrentStateId = Transition.DestinationStateId;
                    Ticket.CurrentState = _db.States.FirstOrDefault(s => s.Id == Ticket.CurrentStateId);
                    string com = DateTime.Now.ToLocalTime().ToString() + ": " + User.Identity.Name.ToString() + " Applied transition (" + Transition.Name + ")";
                    Comment comment = new Comment();
                    comment.Description = com;
                    comment.HistoryId = Ticket.History.Id;
                    await _db.Comments.AddAsync(comment);
                }
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
            return RedirectToPage("./EditAgent", new { id = Ticket.Id });
        }
    }
}
