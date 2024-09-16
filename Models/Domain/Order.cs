using Domain.Identity;
using SportEvents.Domain;

namespace SportEventsAdminApplication.Models.Domain;

public class Order : BaseEntity
{
    public string OwnerId { get; set; }
    public SportEventsAppUser Owner { get; set; }
    public ICollection<TicketInOrder> TicketsInOrder { get; set; }
}