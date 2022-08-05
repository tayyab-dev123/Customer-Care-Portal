namespace CustomerCarePortal.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int HistoryId { get; set; }
        public History History { get; set; }

    }
}
