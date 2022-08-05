using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Workflows
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public CreateModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Workflow Workflow { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Workflows == null || Workflow == null)
            {
                return Page();
            }

            _context.Workflows.Add(Workflow);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
