using Microsoft.Data.SqlClient;

namespace Infrastructure.Interfaces;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}