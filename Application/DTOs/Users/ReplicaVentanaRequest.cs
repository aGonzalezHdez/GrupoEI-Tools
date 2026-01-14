namespace Application.DTOs.Users;

public class ReplicaVentanaRequest
{
    public string UsuarioOrigen { get; set; } = string.Empty;
    public string UsuarioDestino { get; set; } = string.Empty;
}