using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.AdoNet
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

            await connection.OpenAsync();
            int id = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);

            return id > 0;
        }
    }
}
