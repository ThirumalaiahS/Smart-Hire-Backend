using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.AdoNet
{
    public class AdoNetLogRepository : AdoDotNetRepository, ILogRepository
    {
        public AdoNetLogRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }
        public async Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO ErrorLogs(UserId, Endpoint, HttpMethod, StatusCode, ExceptionType, ErrorMessage, StackTrace, RequestPayload, ResponsePayload, CorrelationId, Source, CreatedAt, Severity)
                VALUES(@UserId, @Endpoint, @HttpMethod, @StatusCode, @ExceptionType, @ErrorMessage, @StackTrace, @RequestPayload, @ResponsePayload, @CorrelationId, @Source, @CreatedAt, @Severity)
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@UserId", (object?)errorLogs.UserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Endpoint", errorLogs.Endpoint ?? string.Empty);
            command.Parameters.AddWithValue("@HttpMethod", errorLogs.HttpMethod ?? string.Empty);
            command.Parameters.AddWithValue("@StatusCode", (object?)errorLogs.StatusCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExceptionType", errorLogs.ExceptionType ?? string.Empty);
            command.Parameters.AddWithValue("@ErrorMessage", errorLogs.ErrorMessage ?? string.Empty);
            command.Parameters.AddWithValue("@StackTrace", errorLogs.StackTrace ?? string.Empty);
            command.Parameters.AddWithValue("@RequestPayload", errorLogs.RequestPayload ?? string.Empty);
            command.Parameters.AddWithValue("@ResponsePayload", errorLogs.ResponsePayload ?? string.Empty);
            command.Parameters.AddWithValue("@CorrelationId", (object?)errorLogs.CorrelationId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Source", errorLogs.Source ?? string.Empty);
            command.Parameters.AddWithValue("@CreatedAt", errorLogs.CreatedAt);
            command.Parameters.AddWithValue("@Severity", errorLogs.Severity ?? string.Empty);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
