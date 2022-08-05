using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CustomerCarePortal.Pages.Agents
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;
        [BindProperty]
        public Agent Agent { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null || _db.Departments == null)
            {
                return NotFound();
            }

            var agent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == Id);
            if (agent == null)
            {
                return NotFound();
            }
            Agent = agent;
            ViewData["AgentTeams"] = new SelectList(_db.Teams, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Agent is null)
            {
                return Page();
            }

            try
            {
                var team = _db.Teams.FirstOrDefault(t => t.Id == Agent.TeamId);
                if(team is null)
                {
                    return Page();
                }
                else
                {
                    Agent.Team = team;
                }
                _db.Agents.Update(Agent);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgentExists(Agent.Id))
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

        private bool AgentExists(int id)
        {
            return (_db.Agents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
