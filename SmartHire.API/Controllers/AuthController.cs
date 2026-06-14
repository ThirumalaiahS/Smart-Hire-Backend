using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Core.DTOs;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.Net;

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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                MobileNumber = registerDto.MobileNumber,
                PhoneNumber = registerDto.MobileNumber, 
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) 
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            var resultRole = await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());
            if (!resultRole.Succeeded) 
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(resultRole.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            var userDto = new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user, cancellationToken)
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, statusCode: (int)HttpStatusCode.OK));
        }

        [AllowAnonymous]
        [HttpPost("login")]         
        public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) 
                return Unauthorized(ApiResponse<UserDto>.ErrorResponse(new List<string> { "Invalid email or password" }, statusCode: (int)HttpStatusCode.Unauthorized));

            if (!user.IsActive) 
                return Unauthorized(ApiResponse<UserDto>.ErrorResponse(new List<string> { "Account is deactivated. Please contact support." }, statusCode: (int)HttpStatusCode.Unauthorized));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) 
                return Unauthorized(ApiResponse<UserDto>.ErrorResponse(new List<string> { "Invalid email or password" }, statusCode: (int)HttpStatusCode.Unauthorized));

            var userDto = new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user, cancellationToken)
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, statusCode: (int)HttpStatusCode.OK));
        }

        [Authorize]
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "User not found" }, statusCode: (int)HttpStatusCode.NotFound));

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) 
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Account deactivated successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "User not found" }, statusCode: (int)HttpStatusCode.NotFound));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) 
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "User deleted successfully."));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) 
                return Ok(ApiResponse<object>.SuccessResponse(null!, "If your email is registered, you will receive a password reset link."));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // In a real app, you would send this token via email.
            // For now, we'll return it or just log it.
            
            return Ok(ApiResponse<object>.SuccessResponse(new { Message = "Reset token generated (Simulated email)", Token = token }, "Reset token generated successfully."));  
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) 
                return BadRequest(ApiResponse<object>.ErrorResponse(new List<string> { "Invalid request." }));

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded) 
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Errors.Select(e => e.Description).ToList()));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Password reset successfully."));
        }
    }
}
