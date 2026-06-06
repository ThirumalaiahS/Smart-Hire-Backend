using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.DTOs;
using SmartHire.Core.Interfaces;
using System.Net;

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
        public async Task<IActionResult> GetStats()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var stats = await _repository.GetDashboardStatsAsync(userId);
            return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly()
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var monthly = await _repository.GetMonthlyApplicationsAsync(userId);
            return Ok(ApiResponse<IEnumerable<MonthlyApplicationDto>>.SuccessResponse(monthly));
        }
    }
}
