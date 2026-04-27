namespace Application.DTOs.Users;

public class BajaUsuarioRequest
{
    public IEnumerable<string> ListaUsuarios { get; set; }
    public string MotivoBaja { get; set; }
    public string UsuarioBaja { get; set; } = "11138502189";
}