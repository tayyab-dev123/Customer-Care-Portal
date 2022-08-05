namespace CustomerCarePortal.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int? TicketId { get; set; }
        public Ticket? TicketAssinged { get; set; }
    }
}
