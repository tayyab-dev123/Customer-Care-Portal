using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Teams
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public IndexModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Teams != null)
            {
                Team = await _context.Teams
                .Include(t => t.Department)
                .Include(t => t.TeamManager).ToListAsync();
            }
        }
    }
}
