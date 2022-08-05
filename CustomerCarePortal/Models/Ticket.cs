namespace CustomerCarePortal.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TrackingId { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public History History {get; set;}
        public Team? TeamAssigned { get; set; }
        public Agent? AgentAssigned { get; set; }
        public int? WorkflowId { get; set; }
        public Workflow? Workflow { get; set; }
        public int? CurrentStateId { get; set; }
        public State CurrentState { get; set; }
    }
}
