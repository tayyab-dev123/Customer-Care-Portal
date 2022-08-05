using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Agents
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        public IList<Agent> AllAgentsInSystem { get; set; }
        public ApplicationDbContext _db { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            //Index page is giving error since it is not loading teams with it
            AllAgentsInSystem = _db.Agents.ToList();
            var teams = _db.Teams.ToList();
            foreach (var agent in AllAgentsInSystem)
            {
                var team = teams.Find(t => t.Id == agent.TeamId);
                if (team is not null)
                {
                    agent.Team = team;
                }
            }
        }
    }
}
