using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Velora.Core.Interfaces;

namespace Velora.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public AdminController(IAdminRepository adminRepository, IUserContextService userContextService, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = new Object(); //await _adminRepository.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }
    }
}
    

