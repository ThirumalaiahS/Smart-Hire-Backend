using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Velora.Core.Entities;
using Velora.Core.Interfaces;

namespace Velora.Infrastructure.Repositories.Dapper
{
    public class DapperLogRepository : DapperRepository, ILogRepository
    {
        public DapperLogRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default)
        {
            var sql = @"INSERT INTO ErrorLogs (UserId, Endpoint, HttpMethod, StatusCode, ExceptionType, ErrorMessage, StackTrace, RequestPayload, ResponsePayload, CorrelationId, Source, CreatedAt, Severity)
                        VALUES (@UserId, @Endpoint, @HttpMethod, @StatusCode, @ExceptionType, @ErrorMessage, @StackTrace, @RequestPayload, @ResponsePayload, @CorrelationId, @Source, @CreatedAt, @Severity)";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(new CommandDefinition(sql, errorLogs, cancellationToken: cancellationToken));
        }
    }
}

