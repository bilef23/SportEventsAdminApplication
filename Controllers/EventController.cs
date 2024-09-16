using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using SportEvents.Domain;
using SportEvents.Enum;
using SportEventsAdminApplication.Models;
using SportEventsAdminApplication.Models.Domain;

namespace SportEventsAdminApplication.Controllers;

public class EventController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public EventController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
        string url = "/api/EventsApi/GetAllEvents";
        
        HttpResponseMessage response = await httpClient.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<List<Event>>();
        

        return View(content);
    }

    public async Task<IActionResult> ImportEvents(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            try
            {
                var eventsList = await ProcessExcelFile(file);
                

                HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
                string url = $"/api/EventsApi/ImportEvents";
                
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, eventsList);
                response.EnsureSuccessStatusCode();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error processing file: " + ex.Message);
            }
        }

        return RedirectToAction("Index");
    }
    
    private async Task<List<Event>> ProcessExcelFile(IFormFile file)
    {
        var eventsList = new List<Event>();

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Get the first worksheet
                var rowCount = worksheet.Dimension.Rows;

                // Loop through the rows and read the data (assuming headers on row 1)
                for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip header)
                {
                    var eventTypeString = worksheet.Cells[row, 15].Text;
    
                    // Attempt to parse the string to the EventType enum
                    if (!Enum.TryParse<EventType>(eventTypeString, out var eventType))
                    {
                        // Handle the case where the EventType is invalid or unrecognized
                        throw new Exception($"Invalid EventType '{eventTypeString}' in row {row}");
                    }

                    var eventItem = new Event
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        Location = worksheet.Cells[row, 3].Text,
                        StartDate = DateTime.Parse(worksheet.Cells[row, 4].Text).ToUniversalTime(),
                        EndDate = DateTime.Parse(worksheet.Cells[row, 5].Text).ToUniversalTime(),
                        EventPrice = double.Parse(worksheet.Cells[row, 6].Text),
                        MaximumCapacityEvent = int.Parse(worksheet.Cells[row, 7].Text),
                        MaximumRegistrations = int.Parse(worksheet.Cells[row, 8].Text),
                        OpenForRegistrations = bool.Parse(worksheet.Cells[row, 9].Text),
                        ImageUrl = worksheet.Cells[row, 10].Text,
                        EventType = eventType
                    };
                    var organizer = new Organizer()
                    {
                        Name = worksheet.Cells[row, 11].Text,
                        ContactEmail = worksheet.Cells[row, 12].Text,
                        ContactPhone = worksheet.Cells[row, 13].Text,
                        Address = worksheet.Cells[row, 14].Text
                    };
                    eventItem.Organizer = organizer;
                    eventsList.Add(eventItem);
                }
            }
           
        }

        return eventsList;
    }
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
        string url = $"/api/EventsApi/Details?id={id}";
        
        HttpResponseMessage response = await httpClient.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<Event>();

        return View(content);
    }

        // GET: Event_/Create
        public async Task<IActionResult> Create()
        {
            HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
            string url = $"/api/EventsApi/GetAllOrganizers";
        
            HttpResponseMessage response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadFromJsonAsync<List<Organizer>>();
            var organizers=content.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.Name
            }).ToList();
            
            organizers.Insert(0, new SelectListItem { Value = "", Text = "Please select an organizer" });

            ViewData["OrganizerId"] = organizers;
            return View(new Event());
        }

        // POST: Event_/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                
                HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
                string url = $"/api/EventsApi/Create";
                
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, @event);
                response.EnsureSuccessStatusCode();
                
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
        }

        // GET: Event_/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
            string url = $"/api/EventsApi/Edit?id={id}";
        
            HttpResponseMessage response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadFromJsonAsync<EditPayload>();
            
            
            var organizers = content.Organizers;
            ViewData["OrganizerId"] = new SelectList(organizers, "Id", "Name", content.Event.OrganizerId);
            return View(content.Event);
        }

        // POST: Event_/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event @event)
        {
            
            if (ModelState.IsValid)
            {
                
                HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
                string url = $"/api/EventsApi/Edit";
                
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, @event);
                response.EnsureSuccessStatusCode();
                
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
            
        }

        // GET: Event_/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
            string url = $"/api/EventsApi/Delete?id={id}";
        
            HttpResponseMessage response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadFromJsonAsync<Event>();
            
            
            return View(content);
        }

        // POST: Event_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            HttpClient httpClient = _clientFactory.CreateClient("SportEventManagement");
            string url = $"/api/EventsApi/DeleteConfirmed";

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(url,id);
            response.EnsureSuccessStatusCode();
            
            return RedirectToAction(nameof(Index));
        }
        
}