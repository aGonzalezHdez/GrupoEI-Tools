namespace Application.DTOs.Users;

public class AgregaPantallaMasivoRequest
{
    public IEnumerable<string> ListaUsuarios { get; set; } 
    public IEnumerable<int> ListaIdPantallas { get; set; }
}