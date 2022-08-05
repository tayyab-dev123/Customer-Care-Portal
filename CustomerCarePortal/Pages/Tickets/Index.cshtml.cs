using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerCarePortal.Pages.Tickets
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator,TeamManager,Agent")]
    public class IndexModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;
        public readonly UserManager<IdentityUser> _um;

        public IndexModel(CustomerCarePortal.Data.ApplicationDbContext context, UserManager<IdentityUser> um)
        {
            _um = um;
            _context = context;
        }

        public IList<Ticket> Tickets { get; set; } = default!;
        //flag will be initally 0, for admin 1, for team manager 2, for agent 3
        public int flag { get; set; } = 0;
        public async Task OnGetAsync()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser? currentUser = null;
            if (user is not null)
            {
                currentUser = _um.Users.FirstOrDefault(u => u.Id.Equals(user));
            }
            if (_context.Tickets != null && null != currentUser)
            {
                if (User.IsInRole("Administrator"))
                {
                    flag = 1;
                    Tickets = await _context.Tickets.ToListAsync();
                }
                else if (User.IsInRole("TeamManager"))
                {
                    flag = 2;
                    Tickets = new List<Ticket>();
                    var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Email.Equals(currentUser.Email));
                    if (agent is not null)
                    {
                        var res = await _context.Tickets.FirstOrDefaultAsync(t => t.TeamAssigned.Id == agent.TeamId);
                        if (res is not null)
                            Tickets.Add(res);
                    }

                }
                else if (User.IsInRole("Agent"))
                {
                    flag = 3;
                    Tickets = new List<Ticket>();
                    var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Email.Equals(currentUser.Email));
                    if (agent is not null)
                    {
                        var res = await _context.Tickets.FirstOrDefaultAsync(t => t.AgentAssigned == agent);
                        if (res is not null)
                            Tickets.Add(res);
                    }
                }
            }
        }
    }
}
