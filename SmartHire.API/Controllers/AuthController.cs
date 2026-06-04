using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.DTOs;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            return new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
