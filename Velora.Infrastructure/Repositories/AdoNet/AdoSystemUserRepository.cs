using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Velora.Core.Entities;
using Velora.Core.Interfaces;

namespace Velora.Infrastructure.Repositories.AdoNet
{
    public class AdoSystemUserRepository : AdoDotNetRepository, ISystemUserRepository
    {
        public AdoSystemUserRepository(IConfiguration configuration) : base(configuration)
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
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FullName", systemUser.FullName);
            command.Parameters.AddWithValue("@Email", systemUser.Email);
            command.Parameters.AddWithValue("@Mobile", systemUser.Mobile);
            command.Parameters.AddWithValue("@IsActive", systemUser.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", systemUser.CreatedAt);
            command.Parameters.AddWithValue("@IdentityUserId", systemUser.IdentityUserId);

            await connection.OpenAsync(cancellationToken);
            int id = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);

            return id > 0;
        }

        public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken)
        {
            const string sql = @"
                DELETE FROM SystemUsers WHERE IdentityUserId = @id;
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync(cancellationToken);
            int result = await command.ExecuteNonQueryAsync(cancellationToken);

            return result > 0;
        }

        public async Task<bool> UpdateUserStatus(string id, bool isActive, CancellationToken cancellationToken)
        {
            const string sql = @"
                UPDATE SystemUsers SET IsActive = @isActive WHERE IdentityUserId = @id;
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync(cancellationToken);
            int result = await command.ExecuteNonQueryAsync(cancellationToken);

            return result > 0;
        }
    }
}

