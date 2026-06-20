using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.Data;

namespace SmartHire.Infrastructure.Repositories.AdoNet
{
    public class AdoNetJobApplicationRepository : AdoDotNetRepository, IJobApplicationRepository
    {
        public AdoNetJobApplicationRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                INSERT INTO JobApplications (UserId, CompanyName, JobTitle, Status, AppliedDate, UpdatedAt)
                VALUES (@UserId, @CompanyName, @JobTitle, @Status, @AppliedDate, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", application.UserId);
            command.Parameters.AddWithValue("@CompanyName", application.CompanyName);
            command.Parameters.AddWithValue("@JobTitle", application.JobTitle);
            command.Parameters.AddWithValue("@Status", application.Status.ToString());
            command.Parameters.AddWithValue("@AppliedDate", application.AppliedDate);
            command.Parameters.AddWithValue("@UpdatedAt", application.UpdatedAt);

            await connection.OpenAsync(cancellationToken);
            application.Id = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
            return application;
        }

        public async Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM JobApplications WHERE Id = @Id AND UserId = @UserId";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
        {
            var applications = new List<JobApplication>();
            string sql = "SELECT * FROM JobApplications WHERE UserId = @UserId";
            if (status.HasValue)
            {
                sql += " AND Status = @Status";
            }
            sql += " ORDER BY AppliedDate DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            if (status.HasValue)
            {
                command.Parameters.AddWithValue("@Status", status.Value.ToString());
            }

            await connection.OpenAsync(cancellationToken);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                applications.Add(MapReaderToJobApplication(reader));
            }
            return applications;
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT * FROM JobApplications WHERE Id = @Id AND UserId = @UserId";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync(cancellationToken);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapReaderToJobApplication(reader);
            }
            return null;
        }

        public async Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            application.UpdatedAt = DateTime.UtcNow;
            const string sql = @"
                UPDATE JobApplications 
                SET CompanyName = @CompanyName, JobTitle = @JobTitle, Status = @Status, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", application.Id);
            command.Parameters.AddWithValue("@UserId", application.UserId);
            command.Parameters.AddWithValue("@CompanyName", application.CompanyName);
            command.Parameters.AddWithValue("@JobTitle", application.JobTitle);
            command.Parameters.AddWithValue("@Status", application.Status.ToString());
            command.Parameters.AddWithValue("@UpdatedAt", application.UpdatedAt);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static JobApplication MapReaderToJobApplication(SqlDataReader reader)
        {
            return new JobApplication
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                JobTitle = reader.GetString(reader.GetOrdinal("JobTitle")),
                Status = Enum.Parse<ApplicationStatus>(reader.GetString(reader.GetOrdinal("Status"))),
                AppliedDate = reader.GetDateTime(reader.GetOrdinal("AppliedDate")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
