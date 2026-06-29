using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Velora.Core.DTOs;
using Velora.Core.DTOs.Job;
using Velora.Core.Entities;
using Velora.Core.Interfaces;
using System.Net;

namespace Velora.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IUserContextService _userContext;
        private readonly IMapper _mapper;

        public JobApplicationsController(IJobApplicationRepository repository, IUserContextService userContext, IMapper mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobApplications([FromQuery] ApplicationStatus? status, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var applications = await _repository.GetAllByUserIdAsync(userId, status, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<JobApplicationDTO>>(applications);
            return Ok(ApiResponse<IEnumerable<JobApplicationDTO>>.SuccessResponse(dtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobApplication(int id, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var application = await _repository.GetByIdAsync(id, userId, cancellationToken);

            if (application == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));
            }

            var dto = _mapper.Map<JobApplicationDTO>(application);
            return Ok(ApiResponse<JobApplicationDTO>.SuccessResponse(dto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobApplication(CreateJobApplicationDto createDto, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var application = _mapper.Map<JobApplication>(createDto);
            application.UserId = userId;
            var createdApplication = await _repository.AddAsync(application, cancellationToken);

            var resultDto = _mapper.Map<JobApplicationDTO>(createdApplication);

            return CreatedAtAction(nameof(GetJobApplication), new { id = createdApplication.Id }, ApiResponse<JobApplicationDTO>.SuccessResponse(resultDto, statusCode: (int)HttpStatusCode.Created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobApplication(int id, UpdateJobApplicationDto updateDto, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            // Ensure the user owns the application
            var existing = await _repository.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));

            _mapper.Map(updateDto, existing);
            await _repository.UpdateAsync(existing, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Job application updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(int id, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized(ApiResponse<object>.ErrorResponse(new List<string> { "Unauthorized access" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var existing = await _repository.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "Job application not found" }, statusCode: (int)HttpStatusCode.NotFound));

            await _repository.DeleteAsync(id, userId, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Job application deleted successfully."));
        }
    }
}

