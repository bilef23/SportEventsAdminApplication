using SportEvents.Domain;
using SportEvents.Enum;

namespace SportEventsAdminApplication.Models.Domain;

public class Participant : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; } 
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }
    public ICollection<Registration> Registrations { get; set; }
}