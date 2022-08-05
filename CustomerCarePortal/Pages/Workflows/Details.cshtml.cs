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
    public class DetailsModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public DetailsModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Workflow Workflow { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Workflows == null)
            {
                return NotFound();
            }

            var workflow = await _context.Workflows.FirstOrDefaultAsync(m => m.Id == id);
            if (workflow == null)
            {
                return NotFound();
            }
            else 
            {
                Workflow = workflow;
            }
            return Page();
        }
    }
}
