using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

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
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications([FromQuery] ApplicationStatus? status)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var applications = await _repository.GetAllByUserIdAsync(userId, status);
            return Ok(applications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetJobApplication(int id)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var application = await _repository.GetByIdAsync(id, userId);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        [HttpPost]
        public async Task<ActionResult<JobApplication>> CreateJobApplication(JobApplication application)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            application.UserId = userId;
            var createdApplication = await _repository.AddAsync(application);

            return CreatedAtAction(nameof(GetJobApplication), new { id = createdApplication.Id }, createdApplication);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobApplication(int id, JobApplication application)
        {
            if (id != application.Id)
            {
                return BadRequest();
            }

            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Ensure the user owns the application
            var existing = await _repository.GetByIdAsync(id, userId);
            if (existing == null) return NotFound();

            application.UserId = userId; // Ensure UserId is set to the current user
            await _repository.UpdateAsync(application);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _repository.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}
