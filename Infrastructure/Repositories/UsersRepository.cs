using System.Data;
using System.Data.Common;
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

    public async Task<OperationResult> AgregregaPantallaMasivo(IEnumerable<string> usuarios, IEnumerable<int> pantallas, CancellationToken ct)
    {
        var result = new OperationResult();

        const string sqlGetUser = @"SELECT IdUsuario
                                    FROM CATALOGODEUSUARIOS
                                    WHERE Usuario = @Usuario";

        const string sqlInsert = @"INSERT INTO PANTALLASYREPORTESPORUSUARIO
                                   (IdUsuario, IdPantalla, NivelDeAcceso)
                                   VALUES (@IdUsuario, @IdPantalla, @NivelDeAcceso)";

        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);

        await using var transaction = await conn.BeginTransactionAsync(ct);

        try
        {
            foreach (var usuario in usuarios)
            {
                int? idUsuario = null;

                await using (DbCommand cmdUser = conn.CreateCommand())
                {
                    cmdUser.Transaction = transaction;
                    cmdUser.CommandText = sqlGetUser;
                    cmdUser.CommandType = CommandType.Text;
                    cmdUser.CommandTimeout = _timeout;

                    cmdUser.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 50)
                    {
                        Value = usuario
                    });

                    var value = await cmdUser.ExecuteScalarAsync(ct);

                    if (value != null && value != DBNull.Value)
                        idUsuario = Convert.ToInt32(value);
                }

                if (!idUsuario.HasValue)
                    continue;

                foreach (var pantalla in pantallas)
                {
                    await using DbCommand cmdInsert = conn.CreateCommand();

                    cmdInsert.Transaction = transaction;
                    cmdInsert.CommandText = sqlInsert;
                    cmdInsert.CommandType = CommandType.Text;
                    cmdInsert.CommandTimeout = _timeout;

                    cmdInsert.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int)
                    {
                        Value = idUsuario.Value
                    });

                    cmdInsert.Parameters.Add(new SqlParameter("@IdPantalla", SqlDbType.Int)
                    {
                        Value = pantalla
                    });

                    cmdInsert.Parameters.Add(new SqlParameter("@NivelDeAcceso", SqlDbType.Int)
                    {
                        Value = 4
                    });

                    await cmdInsert.ExecuteNonQueryAsync(ct);
                }
            }

            await transaction.CommitAsync(ct);
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }
    }

    public async Task<OperationResult> DeshabilitarUsuario(IEnumerable<string> usuarios,string motivoBaja,string usuarioBaja, CancellationToken ct)
    {
        var result = new OperationResult();
        const string sqlGetUser = @"SELECT IdUsuario
                                    FROM CATALOGODEUSUARIOS
                                    WHERE Usuario = @UsuarioBaja";
        
        const string sqlUpdate = @"
                            UPDATE CATALOGODEUSUARIOS
                            SET
                                UsuarioActivo = 0,
                                UsuarioBaja = @UsuarioBaja,
                                FechaUsuarioBaja = GETDATE(),
                                MotivoBaja = @MotivoBaja
                            WHERE Usuario = @Usuario;"; 
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var transaction = await conn.BeginTransactionAsync(ct);
        try
        {
            int? idUsuarioBaja = null;

            await using (DbCommand cmdUser = conn.CreateCommand())
            {
                cmdUser.Transaction = transaction;
                cmdUser.CommandText = sqlGetUser;
                cmdUser.CommandType = CommandType.Text;
                cmdUser.CommandTimeout = _timeout;

                cmdUser.Parameters.Add(new SqlParameter("@UsuarioBaja", SqlDbType.VarChar, 50)
                {
                    Value = usuarioBaja
                });

                var value = await cmdUser.ExecuteScalarAsync(ct);

                if (value != null && value != DBNull.Value)
                    idUsuarioBaja = Convert.ToInt32(value);
            }
            
            foreach (var usuario in usuarios)
            {
                await using DbCommand cmdInsert = conn.CreateCommand();
                
                cmdInsert.Transaction = transaction;
                cmdInsert.CommandText = sqlUpdate;
                cmdInsert.CommandType = CommandType.Text;
                cmdInsert.CommandTimeout = _timeout;
                
                cmdInsert.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar, 250)
                {
                    Value = usuario
                });
                
                cmdInsert.Parameters.Add(new SqlParameter("@UsuarioBaja", SqlDbType.Int)
                {
                    Value = idUsuarioBaja
                });
                
                cmdInsert.Parameters.Add(new SqlParameter("@MotivoBaja", SqlDbType.VarChar, 250)
                {
                    Value = motivoBaja
                });
                await cmdInsert.ExecuteNonQueryAsync(ct);
            }
            await transaction.CommitAsync(ct);
            result.Success = true;
            result.Message = "Proceso de update terminado";
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }
    }
}