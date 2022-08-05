using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Identity;

namespace CustomerCarePortal.Pages.Teams
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _um;

        public EditModel(CustomerCarePortal.Data.ApplicationDbContext context, UserManager<IdentityUser> um)
        {
            _um = um;
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; } = default!;
        [BindProperty]
        public int managerId { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Agents)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            Team = team;
            if (Team.TeamManagerId != null)
                managerId = (int)Team.TeamManagerId;
            ViewData["Departments"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["TeamManagers"] = new SelectList(Team.Agents, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Team).State = EntityState.Modified;

            try
            {
                if (Team.TeamManagerId != managerId)
                {
                    var oldManager = await _context.Agents.FindAsync(managerId);
                    var newManager = await _context.Agents.FindAsync(Team.TeamManagerId);
                    if (oldManager is not null && newManager is not null)
                    {
                        var oldManagerUser = _context.Users.FirstOrDefault(u => u.Email.Equals(oldManager.Email));
                        var newManagerUser = _context.Users.FirstOrDefault(u => u.Email.Equals(newManager.Email));
                        if(oldManagerUser is not null && newManagerUser is not null)
                        {
                            await _um.AddToRoleAsync(newManagerUser, "TeamManager");
                            await _um.RemoveFromRoleAsync(oldManagerUser, "TeamManager");
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(Team.Id))
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

        private bool TeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
