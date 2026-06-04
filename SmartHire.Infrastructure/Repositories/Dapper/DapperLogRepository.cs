using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public class DapperLogRepository : DapperRepository, ILogRepository
    {
        public DapperLogRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task CreateErrorLogAsync(ErrorLogs errorLogs)
        {
            var sql = @"INSERT INTO ErrorLogs (UserId, Endpoint, HttpMethod, StatusCode, ExceptionType, ErrorMessage, StackTrace, RequestPayload, ResponsePayload, CorrelationId, Source, CreatedAt, Severity)
                        VALUES (@UserId, @Endpoint, @HttpMethod, @StatusCode, @ExceptionType, @ErrorMessage, @StackTrace, @RequestPayload, @ResponsePayload, @CorrelationId, @Source, @CreatedAt, @Severity)";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, errorLogs);
        }
    }
}
