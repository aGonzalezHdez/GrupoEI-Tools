using Application.Common.Security;
using Application.Interfaces.Reporting;
using Application.Interfaces.Reporting.UseCase;
using Application.Interfaces.Users;
using Application.Interfaces.Users.UseCases;
using Application.Reporting;
using Application.Reporting.UseCase;
using Application.Users;
using Application.Users.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Registration;

public static class AddApplicationServices
{
    public static IServiceCollection ApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //-----Reporting------
        services.AddTransient<IReportingUseCase, ReportingUseCase>();
        services.AddTransient<IVerificacionComprobanteFiscalUseCase, VerificacionComprobanteFiscalUseCase>();
        
        //-----Users----------
        services.AddTransient<IUsersUseCase, UsersUseCase>();
        services.AddTransient<IResetPasswordUseCase, ResetPasswordUseCase>();
        services.AddTransient<IDuplicaVentanaUsuarioUseCase, DuplicaVentanaUsuarioUseCase>();
        services.AddTransient<IActualizaPuestoUseCase, ActualizaPuestoUseCase>();
        
        //-----Extras---------
        services.Configure<SecurityOptions>(configuration.GetSection("Security"));
        return services;
    }
}