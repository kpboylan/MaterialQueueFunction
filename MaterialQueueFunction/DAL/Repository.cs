using MaterialQueueFunction.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MaterialQueueFunction.DAL
{
    public class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly string _connectionString;

        public Repository(ILogger<Repository> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("SqlConnection")
                ?? throw new InvalidOperationException("Missing SqlConnection in configuration.");
        }

        public async Task AddMaterialAsync(Material material)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = new SqlCommand("dbo.InsertMaterial", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@MaterialName", material.MaterialName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MaterialType", material.MaterialType);
                command.Parameters.AddWithValue("@Description", material.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CurrentStock", material.CurrentStock);
                command.Parameters.AddWithValue("@UOMID", material.UOMId);
                command.Parameters.AddWithValue("@Active", material.Active);
                command.Parameters.AddWithValue("@CountryCode", material.CountryCode ?? (object)DBNull.Value);

                await connection.OpenAsync();
                _logger.LogInformation("✅ SQL Server connected successfully.");

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SQL insert failed.");
                throw;
            }
        }
    }
}
