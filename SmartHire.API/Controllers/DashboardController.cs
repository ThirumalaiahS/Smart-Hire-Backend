using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.Interfaces;

namespace SmartHire.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _repository;
        private readonly IUserContextService _userContext;

        public DashboardController(IDashboardRepository repository, IUserContextService userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var stats = await _repository.GetDashboardStatsAsync(userId);
            return Ok(stats);
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<IEnumerable<MonthlyApplicationDto>>> GetMonthly()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var monthly = await _repository.GetMonthlyApplicationsAsync(userId);
            return Ok(monthly);
        }
    }
}
