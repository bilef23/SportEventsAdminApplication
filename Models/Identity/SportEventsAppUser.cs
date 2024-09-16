using Microsoft.AspNetCore.Identity;
using SportEvents.Domain;
using SportEventsAdminApplication.Models.Domain;

namespace Domain.Identity;

public class SportEventsAppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public ShoppingCart? ShoppingCart { get; set; } = new ShoppingCart();
    public ICollection<Order> Orders { get; set; }
}