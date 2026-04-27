using Domain.Enums;

namespace Application.DTOs.Users;

public class PermisoCandadoRequest
{
    public string Usuario { get; set; }
    public CATALOGODETIPOSDEERRORES Candado { get; set; } 
}