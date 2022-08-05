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
    public class DetailsModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public DetailsModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Team Team { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t=>t.Department)
                .Include(t=>t.TeamManager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            else 
            {
                Team = team;
            }
            return Page();
        }
    }
}
