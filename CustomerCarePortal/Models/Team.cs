using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerCarePortal.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Agent>? Agents { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [ForeignKey("TeamManager")]
        public int? TeamManagerId { get; set; }
        public Agent? TeamManager { get; set; }
        public IList<Ticket>? TicketsAssigned { get; set; }
    }
}
