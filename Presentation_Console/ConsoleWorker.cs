using Application.DTOs.Users;
using Application.Interfaces.Reporting;
using Application.Interfaces.Users;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Presentation_Console;

public class ConsoleWorker : BackgroundService
{
    private readonly ILogger<ConsoleWorker> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IUsersUseCase _usersUseCase;
    private readonly IReportingUseCase  _reportingUseCase;
    
    private readonly IValidator<ReplicaVentanaRequest> _duplicaVentanaValidator;
    private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
    private readonly IValidator<ActualizaPuestoRequest> _actualizaPuestoValidator;

    public ConsoleWorker(ILogger<ConsoleWorker> logger, IHostApplicationLifetime lifetime, IUsersUseCase usersUseCase, IReportingUseCase reportingUseCase,
        IValidator<ReplicaVentanaRequest> duplicaVentanaValidator,
        IValidator<ResetPasswordRequest> resetPasswordValidator,
        IValidator<ActualizaPuestoRequest> actualizaPuestoValidator)
    {
        _logger = logger;
        _lifetime = lifetime;
        _usersUseCase = usersUseCase;
        _reportingUseCase = reportingUseCase;
        _duplicaVentanaValidator = duplicaVentanaValidator;
        _resetPasswordValidator = resetPasswordValidator;
        _actualizaPuestoValidator = actualizaPuestoValidator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ShowMenu();

            // Lee una tecla sin requerir Enter
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Q)
            {
                _logger.LogInformation("Saliendo...");
                _lifetime.StopApplication(); // cierre ordenado del Host
                return;
            }

            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    await RunOption1Async(stoppingToken);
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    await RunDuplicaVentanaUsuario(stoppingToken);
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    await RunResetPassword(stoppingToken);
                    break;
                
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    await RunActualizaPuesto(stoppingToken);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Opción inválida.");
                    Pause();
                    break;
            }
        }
    }
    
    private async Task RunOption1Async(CancellationToken ct)
    {
        Console.WriteLine();
        Console.WriteLine("Ejecutando opción 1...");
        try
        {
            Console.Write("Teclee el uuid: ");
            var uuid = Console.ReadLine()?.Trim();
            await _reportingUseCase.VerificacionCfdiAsync(uuid,ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando opción 1.");
        }

        Pause();
    }

    private async Task RunDuplicaVentanaUsuario(CancellationToken ct)
    {
        Console.WriteLine();
        Console.WriteLine("Ejecutando Replica de ventanas...");
        Console.Write("Teclee el numero de usuario origen: ");
        var usuarioOrigen = Console.ReadLine()?.Trim();
        Console.Write("Teclee el numero de usuario destino: ");
        var usuarioDestino = Console.ReadLine()?.Trim();
        
        var request = new ReplicaVentanaRequest
        {
            UsuarioOrigen = usuarioOrigen,
            UsuarioDestino = usuarioDestino
        };
        
        await RunValidatedAsync(request, _duplicaVentanaValidator, async r =>
            {
                var result = await _usersUseCase.DuplicaVentanaUsuario(r.UsuarioOrigen, r.UsuarioDestino, ct);
                Console.WriteLine($"{result.Success} : {result.Message}");
            },ct);
        
        Pause();
    }

    private async Task RunResetPassword(CancellationToken ct)
    {
        Console.WriteLine();
        Console.WriteLine("Reset de password...");
        Console.Write("Teclee el numero de usuario: ");
        var usuario = Console.ReadLine()?.Trim();

        var request = new ResetPasswordRequest()
        {
            Usuario = usuario
        };

        await RunValidatedAsync(request, _resetPasswordValidator, async r =>
            {
                var result = await _usersUseCase.ResetPassword(request.Usuario, ct);
                Console.WriteLine($"{result.Success} : {result.Message}");
            }, ct);
        
        Pause();
    }


    private async Task RunActualizaPuesto(CancellationToken ct)
    {
        Console.WriteLine();
        Console.WriteLine("Actualiza perfil...");
        Console.Write("Teclee el numero de usuario: ");
        var usuario = Console.ReadLine()?.Trim();

        var request = new ActualizaPuestoRequest()
        {
            Usuario = usuario
        };

        await RunValidatedAsync(request,_actualizaPuestoValidator, async request =>
        {
            var result = await _usersUseCase.ActualizaPuesto(request.Usuario, ct);
            Console.WriteLine($"{result.Success} : {result.Message}");
        }, ct);
        
        Pause();
    }
    
    private static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("           GrupoEI Tools (Menu)");
        Console.WriteLine("========================================");
        Console.WriteLine("1) Ejecutar Verificación CFDI (por UUID)");
        Console.WriteLine("2) Replicar ventanas de usuarios");
        Console.WriteLine("3) Reset password");
        Console.WriteLine("4) Actualiza perfil");
        Console.WriteLine();
        Console.WriteLine("Q) Salir");
        Console.WriteLine("========================================");
        Console.Write("Selecciona una opción: ");
    }
    
    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey(intercept: true);
    }
    
    private async Task RunValidatedAsync<TRequest>(
        TRequest request,
        IValidator<TRequest> validator,
        Func<TRequest, Task> handler,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
        {
            Console.WriteLine();
            Console.WriteLine("Errores de validación:");

            foreach (var error in validation.Errors)
            {
                _logger.LogWarning("Validación fallida: {Property} => {Message}", error.PropertyName, error.ErrorMessage);
                Console.WriteLine($"- {error.PropertyName}: {error.ErrorMessage}");
            }

            return;
        }

        try
        {
            await handler(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando comando de consola ({RequestType}).", typeof(TRequest).Name);
            Console.WriteLine("Ocurrió un error al ejecutar la operación. Revisa el log para más detalle.");
        }
    }
}