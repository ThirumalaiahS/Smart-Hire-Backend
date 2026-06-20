using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public class DapperSystemUserRepository : DapperRepository, ISystemUserRepository
    {
        public DapperSystemUserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken)
        {
            const string sql = @"
                INSERT INTO SystemUsers (FullName, Email, Mobile, IsActive, CreatedAt, IdentityUserId)
                VALUES (@FullName, @Email, @Mobile, @IsActive, @CreatedAt, @IdentityUserId);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            using var connection = new SqlConnection(_connectionString);
            systemUser.UserId = await connection.QuerySingleAsync<int>(new CommandDefinition(sql, systemUser, cancellationToken: cancellationToken));
            return systemUser.UserId > 0;
        }

        public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken)
        {
            const string sql = @"
                DELETE FROM SystemUsers WHERE IdentityUserId = @id;
            ";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            return result > 0;
        }

        public async Task<bool> UpdateUserStatus(string id, bool isActive, CancellationToken cancellationToken)
        {
            const string sql = @"
                UPDATE SystemUsers SET IsActive = @isActive WHERE IdentityUserId = @id;
            ";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                new CommandDefinition(sql, new { id, isActive }, cancellationToken: cancellationToken));
            return result > 0;
        }
    }
}
