using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Velora.Core.DTOs;
using Velora.Core.Interfaces;
using System.Net;

namespace Velora.API.Controllers
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
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var stats = await _repository.GetDashboardStatsAsync(userId, cancellationToken);
            return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly(CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var monthly = await _repository.GetMonthlyApplicationsAsync(userId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<MonthlyApplicationDto>>.SuccessResponse(monthly));
        }
    }
}

