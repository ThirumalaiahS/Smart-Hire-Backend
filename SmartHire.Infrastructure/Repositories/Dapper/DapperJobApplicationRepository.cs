using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public class DapperJobApplicationRepository : DapperRepository, IJobApplicationRepository
    {
        public DapperJobApplicationRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<JobApplication> AddAsync(JobApplication application)
        {
            const string sql = @"
                INSERT INTO JobApplications (UserId, CompanyName, JobTitle, Status, AppliedDate, UpdatedAt)
                VALUES (@UserId, @CompanyName, @JobTitle, @Status, @AppliedDate, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            application.Id = await connection.QuerySingleAsync<int>(sql, application);
            return application;
        }

        public async Task DeleteAsync(int id, string userId)
        {
            const string sql = "DELETE FROM JobApplications WHERE Id = @Id AND UserId = @UserId";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null)
        {
            string sql = "SELECT * FROM JobApplications WHERE UserId = @UserId";
            if (status.HasValue)
            {
                sql += " AND Status = @Status";
            }
            sql += " ORDER BY AppliedDate DESC";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<JobApplication>(sql, new { UserId = userId, Status = status?.ToString() });
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId)
        {
            const string sql = "SELECT * FROM JobApplications WHERE Id = @Id AND UserId = @UserId";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<JobApplication>(sql, new { Id = id, UserId = userId });
        }

        public async Task UpdateAsync(JobApplication application)
        {
            application.UpdatedAt = DateTime.UtcNow;
            const string sql = @"
                UPDATE JobApplications 
                SET CompanyName = @CompanyName, JobTitle = @JobTitle, Status = @Status, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, application);
        }
    }
}
