using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Velora.Core.Interfaces;

namespace Velora.Infrastructure.Repositories.Dapper
{
    public class DapperDashboardRepository : DapperRepository, IDashboardRepository
    {
        public DapperDashboardRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId, CancellationToken cancellationToken = default)
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

            await using var connection = new SqlConnection(_connectionString);
            var row = await connection.QuerySingleAsync(new CommandDefinition(sql, new { userId }, cancellationToken: cancellationToken));

            int total = (int)(row.Total ?? 0);
            int active = (int)(row.Active ?? 0);
            int interviews = (int)(row.Interviews ?? 0);
            int offers = (int)(row.Offers ?? 0);
            int rejected = (int)(row.Rejected ?? 0);
            double responseRate = (double)(row.ResponseRate ?? 0.0);

            return new DashboardStatsDto(total, active, interviews, offers, rejected, Math.Round(responseRate, 1));     
        }

        public async Task<IEnumerable<MonthlyApplicationDto>> GetMonthlyApplicationsAsync(string userId, CancellationToken cancellationToken = default)
        {
            const string sql = @"
                SELECT FORMAT(AppliedDate,'MMM yyyy') AS Month,
                    COUNT(*) AS Count,
                    SUM(CASE WHEN Status='Interview' THEN 1 ELSE 0 END) AS Interviews
                FROM JobApplications
                WHERE UserId = @userId AND AppliedDate >= DATEADD(MONTH,-6,GETUTCDATE())
                GROUP BY FORMAT(AppliedDate,'MMM yyyy'), YEAR(AppliedDate), MONTH(AppliedDate)
                ORDER BY YEAR(AppliedDate), MONTH(AppliedDate)";
            await using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<MonthlyApplicationDto>(new CommandDefinition(sql, new { userId }, cancellationToken: cancellationToken));
        }
    }
}

