using Microsoft.AspNetCore.Components.Server.Circuits;
using Quizor.Code;
using Quizor.Shared;

namespace Quizor;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor(options =>
        {
            options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(1);
            options.DisconnectedCircuitMaxRetained = 200;
        });
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