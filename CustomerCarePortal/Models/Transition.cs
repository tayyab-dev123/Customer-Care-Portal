using System.ComponentModel;

namespace CustomerCarePortal.Models
{
    public class Transition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SourceStateId { get; set; }
        [DisplayName("Source State")]
        public State SourceState { get; set; }
        public int DestinationStateId { get; set; }
        [DisplayName("Destination State")]
        public State DestinationState { get; set; }
    }
}
