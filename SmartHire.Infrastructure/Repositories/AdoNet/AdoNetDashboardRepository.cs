using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories.AdoNet
{
    public class AdoNetDashboardRepository : AdoDotNetRepository, IDashboardRepository
    {
        public AdoNetDashboardRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
        {
             const string sql = @"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN Status IN ('Rejected','Withdrawn','Ghosted') THEN 1 ELSE 0 END) AS Active,
                    SUM(CASE WHEN Status = 'Interview' THEN 1 ELSE 0 END) AS Interviews,
                    SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,
                    SUM(CASE WHEN Status = 'Offer' THEN 1 ELSE 0 END) AS Offers,
                    CASE 
                        WHEN COUNT(*) = 0 THEN 0 
                        ELSE CAST(SUM(CASE WHEN Status IN ('Rejected','Withdrawn','Ghosted') THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100 
                    END AS ResponseRate 
                FROM JobApplications WHERE UserId = @userId;
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int total = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                int active = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                int interviews = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                int rejected = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                int offers = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                double responseRate = reader.IsDBNull(5) ? 0 : reader.GetDouble(5);

                return new DashboardStatsDto(total, active, interviews, offers, rejected, Math.Round(responseRate, 1));
            }

            return new DashboardStatsDto(0, 0, 0, 0, 0, 0);
        }

        public async Task<IEnumerable<MonthlyApplicationDto>> GetMonthlyApplicationsAsync(string userId)
        {
            var results = new List<MonthlyApplicationDto>();
            const string sql = @"
                SELECT FORMAT(AppliedDate,'MMM yyyy') AS Month,
                    COUNT(*) AS Count,
                    SUM(CASE WHEN Status='Interview' THEN 1 ELSE 0 END) AS Interviews
                FROM JobApplications
                WHERE UserId = @userId AND AppliedDate >= DATEADD(MONTH,-6,GETUTCDATE())
                GROUP BY FORMAT(AppliedDate,'MMM yyyy'), YEAR(AppliedDate), MONTH(AppliedDate)
                ORDER BY YEAR(AppliedDate), MONTH(AppliedDate)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new MonthlyApplicationDto(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2)
                ));
            }
            return results;
        }
    }
}
