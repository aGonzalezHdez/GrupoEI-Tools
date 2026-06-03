namespace Application.DTOs.Reporting;

public class CargarControlResultado
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Mensaje { get; set; }

    /// <summary>
    /// Contenido devuelto por el servicio externo. Si la respuesta es JSON se
    /// expone como objeto; en caso contrario se conserva el texto crudo.
    /// </summary>
    public object? Datos { get; set; }
}
