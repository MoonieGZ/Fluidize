using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;

namespace Fluidize;

public static class Program
{
    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File("Logs/latest-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder();

            builder.Host.UseSerilog();

            builder.Services.AddAuthorization();
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

            var app = builder.Build();
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapGet("/health", () => Results.Ok());
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}