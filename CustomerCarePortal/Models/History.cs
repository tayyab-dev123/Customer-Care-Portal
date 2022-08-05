namespace CustomerCarePortal.Models
{
    public class History
    {
        public int Id { get; set; }
        public IList<Comment> Comments { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
