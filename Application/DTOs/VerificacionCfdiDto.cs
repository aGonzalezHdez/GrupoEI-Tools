namespace Application.DTOs;

public class VerificacionCfdiDto
{
    public string? IdReferencia { get; set; }
    public string? ConsFact { get; set; }
    public string? Emisor { get; set; }
    public string? Receptor { get; set; }
    public decimal? Valor { get; set; }
    public string? Uuid { get; set; }
    public string? CodigoEstatus { get; set; }
    public string? EsCancelable { get; set; }
    public string? Estado { get; set; }
    public string? EstatusCancelacion { get; set; }
    public string? ValidacionEfos { get; set; }
    public string? NombreEmisor { get; set; }
    public string? NombreReceptor { get; set; }
    public DateTime? FechaEmision { get; set; }
    public DateTime? FechaSat { get; set; }
    public string? RfcProvCertif { get; set; }
    public string? TipoDeComprobante { get; set; }
}