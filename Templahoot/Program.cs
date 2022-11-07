using Microsoft.AspNetCore.Components.Server.Circuits;
using Templahoot.Code;
using Templahoot.Shared;

namespace Templahoot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddScoped<AttendeeService>();
        builder.Services.AddScoped<CircuitHandler, TrackingCircuitHandler>();
        builder.Services.AddHostedSingleton<CircuitTracker>();
        builder.Services.AddSingleton<QuizInfo>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}