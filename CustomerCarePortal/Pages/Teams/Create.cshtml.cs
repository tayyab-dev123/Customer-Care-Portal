using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Teams
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
        ViewData["Departments"] = new SelectList(_context.Departments, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Team Team { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Teams == null || Team == null)
            {
                return Page();
            }

            _context.Teams.Add(Team);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
