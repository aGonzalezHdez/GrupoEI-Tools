using Application.Registration;
using FluentValidation;
using Infrastructure.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Presentation_Console;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: "logs/grupoei-tools.log",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        using var host = Host.CreateDefaultBuilder(args).UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Registramos capas
                services.ApplicationServices(context.Configuration);
                services.InfrastructureServices(context.Configuration);
                
                services.AddValidatorsFromAssembly(typeof(Application.Users.UsersUseCase).Assembly);

                // Un "runner" de consola
                services.AddHostedService<ConsoleWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}