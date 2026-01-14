using System.Data;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Infrastructure.Interfaces;
using Infrastructure.SQLServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class VerificacionCfdiRepository : IVerificacionCfdiRepository
{
    private readonly ISqlConnectionFactory _factory;
    private readonly int _timeout;
    
    public VerificacionCfdiRepository(ISqlConnectionFactory factory,IOptions<SqlOptions> options)
    {
        _factory = factory;
        _timeout = options.Value.CommandTimeoutSeconds;
    }
    
    public async Task<VerificacionCfdiDto?> GetByUuidAsync(string uuid, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(StoredProcedures.Reporting.VerificacionCfdi, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = _timeout
        };

        cmd.Parameters.Add("@UUID", SqlDbType.VarChar, 36).Value = uuid;

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return null;

        return new VerificacionCfdiDto
        {
            IdReferencia = reader["IdReferencia"] as string,
            ConsFact = reader["ConsFact"] as string,
            Emisor = reader["Emisor"] as string,
            Receptor = reader["Receptor"] as string,
            Valor = reader["Valor"] == DBNull.Value ? null : Convert.ToDecimal(reader["Valor"]),
            Uuid = reader["UUID"] as string,
            CodigoEstatus = reader["CodigoEstatus"] as string,
            EsCancelable = reader["EsCancelable"] as string,
            Estado = reader["Estado"] as string,
            EstatusCancelacion = reader["EstatusCancelacion"] as string,
            ValidacionEfos = reader["ValidacionEFOS"] as string,
            NombreEmisor = reader["NombreEmisor"] as string,
            NombreReceptor = reader["NombreReceptor"] as string,
            FechaEmision = reader["FechaEmision"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaEmision"]),
            FechaSat = reader["FechaSAT"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaSAT"]),
            RfcProvCertif = reader["RfcProvCertif"] as string,
            TipoDeComprobante = reader["TipoDeComprobante"] as string
        };
    }
}