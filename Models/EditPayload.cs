using SportEvents.Domain;
using SportEventsAdminApplication.Models.Domain;

namespace SportEventsAdminApplication.Models;

public class EditPayload
{
    public Event Event { get; set; }
    public List<Organizer> Organizers { get; set; }
}