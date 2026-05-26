using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(string userId);
        Task<IEnumerable<MonthlyApplicationDto>> GetMonthlyApplicationsAsync(string userId);
    }
    public record DashboardStatsDto(
        int Total, int Active, int Interviews,
        int Offers, int Rejected, double ResponseRate);

    public record MonthlyApplicationDto(string Month, int Count, int Interviews);
}
