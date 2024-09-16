using System.Text.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(name: "SportEventManagement")
    .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5042");
            client.Timeout = TimeSpan.FromSeconds(30);
        }
    );
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
var policy = HttpPolicyExtensions.HandleTransientHttpError()
    .Or<ArgumentOutOfRangeException>()
    .WaitAndRetryAsync(5,retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
        retryAttempt)), (exception, timeSpan, retryCount, context) =>
    {
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();