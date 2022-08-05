using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerCarePortal.Areas.Identity.Pages.Roles
{
/*    [Authorize(Roles = "Administrator")]
*/    public class IndexModel : PageModel
    {
        public RoleManager<IdentityRole> _roleManager;
        public List<IdentityRole> AllRoles { get; set; }

        [BindProperty]
        public IdentityRole NewRole { get; set; }
        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public void OnGet()
        {
            AllRoles = _roleManager.Roles.ToList();
        }

        public async Task<IActionResult> OnPostAddRole()
        {
            await _roleManager.CreateAsync(NewRole);

            AllRoles = _roleManager.Roles.ToList();

            return Page();
        }
    }
}
