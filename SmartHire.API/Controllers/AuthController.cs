using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                MobileNumber = registerDto.MobileNumber,
                PhoneNumber = registerDto.MobileNumber, // Also sync with Identity PhoneNumber
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var resultRole = await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());
            if (!resultRole.Succeeded) return BadRequest(resultRole.Errors);
            return new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")] 
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid email or password");

            if (!user.IsActive) return Unauthorized("Account is deactivated. Please contact support.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            return new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user)
            };
        }

        [Authorize]
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Could not deactivate account.");

            return Ok("Account deactivated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest("Could not delete user.");

            return Ok("User deleted successfully.");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) return Ok("If your email is registered, you will receive a password reset link.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // In a real app, you would send this token via email.
            // For now, we'll return it or just log it.
            
            return Ok(new { Message = "Reset token generated (Simulated email)", Token = token });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return BadRequest("Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password reset successfully.");
        }
    }
}
