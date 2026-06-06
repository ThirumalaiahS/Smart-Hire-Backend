using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.DTOs;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.Net;

namespace SmartHire.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IUserContextService _userContext;

        public JobApplicationsController(IJobApplicationRepository repository, IUserContextService userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobApplications([FromQuery] ApplicationStatus? status)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var applications = await _repository.GetAllByUserIdAsync(userId, status);
            return Ok(ApiResponse<IEnumerable<JobApplication>>.SuccessResponse(applications));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobApplication(int id)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var application = await _repository.GetByIdAsync(id, userId);

            if (application == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));
            }

            return Ok(ApiResponse<JobApplication>.SuccessResponse(application));
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobApplication(JobApplication application)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            application.UserId = userId;
            var createdApplication = await _repository.AddAsync(application);

            return CreatedAtAction(nameof(GetJobApplication), new { id = createdApplication.Id }, ApiResponse<JobApplication>.SuccessResponse(createdApplication, statusCode: (int)HttpStatusCode.Created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobApplication(int id, JobApplication application)
        {
            if (id != application.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(new List<string> { "ID mismatch" }));
            }

            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            // Ensure the user owns the application
            var existing = await _repository.GetByIdAsync(id, userId);
            if (existing == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));

            application.UserId = userId; // Ensure UserId is set to the current user
            await _repository.UpdateAsync(application);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Job application updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var existing = await _repository.GetByIdAsync(id, userId);
            if (existing == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));

            await _repository.DeleteAsync(id, userId);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Job application deleted successfully."));
        }
    }
}
