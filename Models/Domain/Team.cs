using SportEventsAdminApplication.Models.Domain;

namespace SportEvents.Domain;

public class Team : BaseEntity
{
    public string Name { get; set; }
    public string CoachName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string City { get; set; }
    public ICollection<Participant> Participants { get; set; }
}