namespace CustomerCarePortal.Models
{
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? IntialStateId { get; set; }
        public IList<State>? States { get; set; }
        public IList<Ticket>? Tickets { get; set; }
    }
}
