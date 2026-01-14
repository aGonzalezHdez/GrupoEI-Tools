using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.ConnectionFactory;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.SQLServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Registration;

public static class AddInfrastructureServices
{
    public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SqlOptions>()
            .Bind(configuration.GetSection("Sql"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString),
                "Sql:ConnectionString es requerida")
            .Validate(o => o.CommandTimeoutSeconds > 0,
                "Sql:CommandTimeoutSeconds debe ser mayor a 0")
            .ValidateOnStart();
        
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IVerificacionCfdiRepository, VerificacionCfdiRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        return services;
    }
}