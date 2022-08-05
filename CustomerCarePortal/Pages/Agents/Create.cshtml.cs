using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCarePortal.Pages.Agents
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        public IList<IdentityUser> AllUsersInSystem { get; set; } 
        public IList<Team> AllTeamsInSystem { get; set; } = default!;
        [BindProperty]
        public int TeamId { get; set; }
        [BindProperty]
        public string UserId { get; set; }

        public UserManager<IdentityUser> _usermanager;
        private ApplicationDbContext _db;
        public CreateModel(UserManager<IdentityUser> usermanager, ApplicationDbContext db)
        {
            _usermanager = usermanager;
            _db = db;
        }
        public void OnGetAsync()
        {
            AllUsersInSystem =  _usermanager.Users.ToList();
            AllTeamsInSystem = _db.Teams.ToList();
        }

        public async Task<IActionResult> OnPostCreateAgent()
        {
            var user = _usermanager.Users.First(user => user.Id == UserId);
            var team = _db.Teams.FirstOrDefault(team => team.Id == TeamId);
            if (user is not null && team is not null)
            {
                Agent agent = new Agent();
                agent.Name = user.UserName.Split('@')[0];
                agent.Team = team;
                agent.TeamId = team.Id;
                agent.Email = user.Email;
                _db.Agents.Add(agent);
                await _usermanager.AddToRoleAsync(user, "Agent");
                await _db.SaveChangesAsync();
            }
            return Redirect("/Agents/Index");
        }
    }
}
