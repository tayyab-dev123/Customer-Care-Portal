using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerCarePortal.Models
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Transition>? Transitions { get; set; }
        [ForeignKey("Workflow")]
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
    }
}
