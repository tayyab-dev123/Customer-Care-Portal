using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerCarePortal.Areas.Identity.Pages.Roles
{
/*    [Authorize(Roles = "Administrator")]
*/    public class DetailsModel : PageModel
    {
        public IList<IdentityUser> AllUsersInSystem { get; set; }
        public IList<IdentityUser> AllUsersInThisRole { get; set; }
        public IdentityRole SelectedRole { get; set; }

        public UserManager<IdentityUser> _usermanager;
        public RoleManager<IdentityRole> _rolemanager;

        public DetailsModel(UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        {
            _rolemanager = rm;
            _usermanager = um;
        }




        public async Task<IActionResult> OnGet(string id)
        {
            SelectedRole = _rolemanager.FindByIdAsync(id).Result;


            AllUsersInSystem = _usermanager.Users.ToList();
            AllUsersInThisRole = _usermanager.GetUsersInRoleAsync(SelectedRole.Name).Result;


            return Page();

        }
        public async Task<IActionResult> OnPostAsync(string TaskType, string uid, string rid)
        {

            IdentityRole currentWorkingRole = _rolemanager.FindByIdAsync(rid).Result;
            IdentityUser currentWorkingUser = _usermanager.FindByIdAsync(uid).Result;



            if (TaskType == "AddUser")
            {
                var k = _usermanager.AddToRoleAsync(currentWorkingUser, currentWorkingRole.Name).Result;
            }
            if (TaskType == "RemoveUser")
            {
                var k = _usermanager.RemoveFromRoleAsync(currentWorkingUser, currentWorkingRole.Name).Result;
            }


            return RedirectToAction("", new
            {
                id = rid
            });
        }
    }
}
