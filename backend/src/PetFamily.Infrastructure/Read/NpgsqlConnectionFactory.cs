using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PetFamily.Infrastructure.Read;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured. Use user-secrets for local development.");
    }

    public DbConnection Create() => new NpgsqlConnection(_connectionString);
}
