using Application.DTOs.Users;
using Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace Presentatio_APIs.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersUseCase _usersUseCase;

    public UsersController(IUsersUseCase usersUseCase)
    {
        _usersUseCase = usersUseCase;
    }
    
    
    [HttpPost("/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        var result = await _usersUseCase.ResetPassword(request.Usuario, ct);
        if (result.Success)
            return Ok(result.Message);
       
        return StatusCode(StatusCodes.Status500InternalServerError,new { mensaje = result.Message });
    }
    
    [HttpPost("/replica-ventanas")]
    public async Task<IActionResult> ReplicaVentana([FromBody] ReplicaVentanaRequest request, CancellationToken ct)
    {
        var result = await _usersUseCase.DuplicaVentanaUsuario(
            request.UsuarioOrigen,
            request.UsuarioDestino,
            ct);

        return result.Success ? Ok() : BadRequest(result.Message);
    }

    [HttpPost("/ActualizaPuesto")]
    public async Task<IActionResult> ActualizaPuesto([FromBody] ActualizaPuestoRequest request, CancellationToken ct)
    {
        var result = await _usersUseCase.ActualizaPuesto(request.Usuario, ct);
        return result.Success ? Ok(result.Message) : StatusCode(StatusCodes.Status500InternalServerError,new { mensaje = result.Message});
    }
    
    
}