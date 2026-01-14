using Infrastructure.Interfaces;
using Infrastructure.SQLServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.ConnectionFactory;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<SqlOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public SqlConnection Create() => new SqlConnection(_connectionString);
}