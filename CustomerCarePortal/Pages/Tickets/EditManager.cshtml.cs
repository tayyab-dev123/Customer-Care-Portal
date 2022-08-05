using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CustomerCarePortal.Pages.Tickets
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "TeamManager")]
    public class EditManagerModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditManagerModel(ApplicationDbContext context)
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
                .Include(d => d.AgentAssigned)
                .Include(d => d.CurrentState)
                .Include(d => d.Workflow)
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
            //All agents who are in the team assigned to ticket and is not a manager
            var validAgents = await _db.Agents
                .Where(a => a.TeamId == Ticket.TeamAssigned.Id)
                .Where(a => a.TicketAssinged == null || a.TicketAssinged.Id == Ticket.Id)
                .Where(a => a.Id != Ticket.TeamAssigned.TeamManagerId)
                .Select(s => s).ToListAsync();
            ViewData["Agents"] = new SelectList(validAgents, "Id", "Name");
            ViewData["Workflows"] = new SelectList(_db.Workflows, "Id", "Name");
            return Page();
        }
        //OnPost Handler

        public async Task<IActionResult> OnPostManagerHandler()
        {
            if (Ticket is null)
            {
                return Page();
            }

            try
            {
                Ticket.TeamAssigned = _db.Teams.FirstOrDefault(t => t.Id == Ticket.TeamAssigned.Id);
                //Removing oldAgent ticket and adding new one
                var oldAgent = await _db.Agents
                    .Where(a => a.TicketAssinged != null && a.TicketAssinged.Id == Ticket.Id)
                    .Select(s => s).ToListAsync();
                if (oldAgent is not null && Ticket.AgentAssigned is not null)
                {
                    Agent res = oldAgent[0];
                    //If the agent is changed then update
                    if (res.Id != Ticket.AgentAssigned.Id)
                    {
                        res.TicketAssinged = null;
                        _db.Agents.Update(res);
                    }
                }
                Ticket.AgentAssigned = _db.Agents.FirstOrDefault(a => a.Id == Ticket.AgentAssigned.Id);
                //Attaching workflow
                Ticket.Workflow = _db.Workflows.FirstOrDefault(w => w.Id == Ticket.WorkflowId);
                if (Ticket.CurrentStateId is not null)
                {
                    Ticket.CurrentState = _db.States.FirstOrDefault(s => s.Id == Ticket.CurrentStateId);
                }
                else if (Ticket.Workflow is not null)
                {
                    Ticket.CurrentStateId = Ticket.Workflow.IntialStateId;
                    Ticket.CurrentState = _db.States.FirstOrDefault(s => s.Id == Ticket.CurrentStateId);
                    _db.Tickets.Update(Ticket);
                    await _db.SaveChangesAsync();
                }
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
            return RedirectToPage("./EditManager", new { id = Ticket.Id });
        }
    }
}
