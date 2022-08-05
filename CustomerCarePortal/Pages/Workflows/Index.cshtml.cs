using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Workflows
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public IndexModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Workflow> Workflow { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Workflows != null)
            {
                Workflow = await _context.Workflows.ToListAsync();
            }
        }
    }
}
