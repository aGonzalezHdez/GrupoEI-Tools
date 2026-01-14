using Application.Registration;
using Application.Users;
using FluentValidation;
using Infrastructure.Registration;
using FluentValidation.AspNetCore;
using Serilog;

namespace Presentatio_APIs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: "logs/grupoei-tools.log",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.ApplicationServices(builder.Configuration);
        builder.Services.InfrastructureServices(builder.Configuration);
        
        //-------Agregamos Fluent Validator-------
        builder.Services.AddValidatorsFromAssembly(typeof(UsersUseCase).Assembly);
        builder.Services.AddFluentValidationAutoValidation();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}