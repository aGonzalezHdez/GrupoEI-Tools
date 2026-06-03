using Application.Interfaces;
using Application.Interfaces.Reporting;
using Application.Interfaces.Repositories;
using Infrastructure.ConnectionFactory;
using Infrastructure.Gateways;
using Infrastructure.Http;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.SQLServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        //-----Servicios externos (HTTP)-----
        services.AddOptions<TokenOptions>()
            .Bind(configuration.GetSection("Token"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl),
                "Token:BaseUrl es requerida")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username),
                "Token:Username es requerido")
            .Validate(o => !string.IsNullOrWhiteSpace(o.PasssEncryp),
                "Token:PasssEncryp es requerido")
            .ValidateOnStart();

        services.AddHttpClient<ITokenProvider, TokenProvider>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<TokenOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
        });

        services.AddOptions<ControlAsignacionOptions>()
            .Bind(configuration.GetSection("ControlAsignacion"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl),
                "ControlAsignacion:BaseUrl es requerida")
            .ValidateOnStart();

        services.AddHttpClient<IControlAsignacionGateway, ControlAsignacionGateway>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<ControlAsignacionOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
        });

        return services;
    }
}