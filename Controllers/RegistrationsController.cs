using Microsoft.AspNetCore.Mvc;
using SportEventsAdminApplication.Models.Domain;

namespace SportEventsAdminApplication.Controllers;

public class RegistrationsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public RegistrationsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // GET
    public async  Task<IActionResult> Index()
    {
        HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
        string url = "/api/RegistrationsApi/GetAllRegistrations";
        
        HttpResponseMessage response = await httpClient.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<List<Registration>>();
        

        return View(content);
    }

    public async Task<IActionResult> ApproveRegistration(Guid id)
    {
        HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
        string url = $"/api/RegistrationsApi/Approve";

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url,id);
        response.EnsureSuccessStatusCode();
            
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> RejectRegistration(Guid id)
    {
        HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
        string url = $"/api/RegistrationsApi/Reject";

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url,id);
        response.EnsureSuccessStatusCode();
            
        return RedirectToAction(nameof(Index));
    }
}