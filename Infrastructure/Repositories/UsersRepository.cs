using System.Data;
using Application.Interfaces.Repositories;
using Infrastructure.Interfaces;
using Infrastructure.SQLServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Application.DTOs;

namespace Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ISqlConnectionFactory _factory;
    private readonly int _timeout;
    
    public UsersRepository(ISqlConnectionFactory factory,IOptions<SqlOptions> options)
    {
        _factory = factory;
        _timeout = options.Value.CommandTimeoutSeconds;
    }

    public async Task<OperationResult> ResetPassword(string user,string defaultPassword,CancellationToken ct)
    {
        var result = new OperationResult();
        const string sql = @"UPDATE CATALOGODEUSUARIOS
                            SET SolicitarCambioDePassword = 1,
                            Psw = @PasswordHash
                            WHERE [Usuario] = @User;";

        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = _timeout;
        
        cmd.Parameters.Add(new SqlParameter("@User", SqlDbType.VarChar, 50)
        {
            Value = user
        });
        
        cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 255)
        {
            Value = defaultPassword
        });
        try
        {
            var rowsAffected = await cmd.ExecuteNonQueryAsync(ct);
            result.Success = true;
            result.Message = "Se ha reseteado correctamente el password";
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"ERROR SQL: {ex.GetType().Name} - {ex.Message}");
            result.Success = false;
            result.Message = $"Error:{ex.Message}";
        }

        return result;
    }
    
    public async Task<OperationResult> DuplicaVentanaUsuarios(string usuarioOrigen, string usuarioDestino, CancellationToken ct)
    {
        var result = new OperationResult();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(StoredProcedures.Usuarios.DuplicaVentanaUsuario, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = _timeout
        };
        var usuarioOrigenId = await ObtenerIdUsuario(conn,usuarioOrigen, ct);
        var usuarioDestinoId = await ObtenerIdUsuario(conn,usuarioDestino, ct);
        cmd.Parameters.Add("@IdUsuarioActual", SqlDbType.Int).Value = usuarioOrigenId;
        cmd.Parameters.Add("@IdUsuarioNuevo", SqlDbType.Int).Value = usuarioDestinoId;
        
        try
        {
            await cmd.ExecuteReaderAsync(ct);
            result.Success = true;
            result.Message = "Se daplicado correctamente";
        }
        catch (SqlException e)
        {
            result.Success = false;
            result.Message = e.Message;
        }

        return result;
    }

    private async Task<int?> ObtenerIdUsuario(SqlConnection conn, string usuario,CancellationToken ct)
    {
        const string sql = @"Select IdUsuario From CATALOGODEUSUARIOS Where Usuario = @Usuario;";
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = _timeout;
        
        cmd.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50)
        {
            Value = usuario
        });
        try
        {
            var result = await cmd.ExecuteScalarAsync(ct);
            if (result == null || result == DBNull.Value)
                return null;
            return Convert.ToInt32(result);
        }
        catch (SqlException e)
        {
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<OperationResult> ActualizaPuesto(string usuario, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(StoredProcedures.Usuarios.ActualizaPuesto, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = _timeout
        };
        var usuarioId = await ObtenerIdUsuario(conn,usuario, ct);
        cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = usuarioId;
        var result = new OperationResult();
        try
        {
            await cmd.ExecuteReaderAsync(ct);
            result.Success = true;
            result.Message = "Puesto actualizado exitosamente";
        }
        catch (SqlException e)
        {
            result.Success = true;
            result.Message = e.Message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return result;
    }
}