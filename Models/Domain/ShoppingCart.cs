using Domain.Identity;
using SportEvents.Domain;

namespace SportEventsAdminApplication.Models.Domain;

public class ShoppingCart : BaseEntity
{
    public string? OwnerId { get; set; }
    public SportEventsAppUser? Owner { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }

}