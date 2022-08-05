namespace CustomerCarePortal.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Team>? Teams { get; set; }
        public int? DepartmentManagerId { get; set; }
        public Agent? DepartmentManager { get; set; }
    }
}
