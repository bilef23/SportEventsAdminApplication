using System.Data.SqlTypes;
using SportEvents.Domain;
using SportEventsAdminApplication.Models.Domain;

namespace SportEventsAdminApplication.Models;

public class EventOrganizerViewModel
{
    public Organizer? Organizer { get; set; }
    public Event Event { get; set; }

    public bool IsOrganizerNull()
    {
        if (Organizer == null || 
            (string.IsNullOrWhiteSpace(Organizer.Name) &&
             string.IsNullOrWhiteSpace(Organizer.ContactEmail) &&
             string.IsNullOrWhiteSpace(Organizer.Address) &&
             string.IsNullOrWhiteSpace(Organizer.ContactPhone)))
        {
            return true;
        }

        return false;
    }
}