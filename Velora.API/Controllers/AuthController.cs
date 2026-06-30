using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Velora.Core.DTOs;
using Velora.Core.Entities;
using Velora.Core.Interfaces;

namespace Velora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly ISystemUserRepository _systemUserRepository;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            ISystemUserRepository systemUserRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _systemUserRepository = systemUserRepository;
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
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(
                    result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            var resultRole = await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());
            if (!resultRole.Succeeded)
            {
                await RollbackRegistration(user);
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(
                    resultRole.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));
            }

            var sysUser = CreateSystemUser(user);
            bool resultSysUser;
            try
            {
                resultSysUser = await _systemUserRepository.CreateUser(sysUser, cancellationToken);
            }
            catch
            {
                await RollbackRegistration(user, registerDto.Role.ToString());
                throw;
            }

            if (!resultSysUser)
            {
                await RollbackRegistration(user, registerDto.Role.ToString());
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(
                    new List<string> { "Unable to create system user due to an unknown error" },
                    statusCode: (int)HttpStatusCode.BadRequest));
            }

            var userDto = new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user, cancellationToken)
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, message:"Account created successfully!", statusCode: (int)HttpStatusCode.OK));
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

            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var userDto = await _tokenService.GenerateTokens(user, ipAddress, cancellationToken);

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, statusCode: (int)HttpStatusCode.OK));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring(7).Trim()
                : authHeader;
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            try
            {
                var userDto = await _tokenService.RefreshToken(token, request.RefreshToken, ipAddress, cancellationToken);
                return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, statusCode: (int)HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<UserDto>.ErrorResponse(new List<string> { ex.Message }, statusCode: (int)HttpStatusCode.Unauthorized));
            }
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var result = await _tokenService.RevokeToken(request.RefreshToken, ipAddress, cancellationToken);
            if (!result)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(new List<string> { "Invalid or already revoked refresh token." }, statusCode: (int)HttpStatusCode.BadRequest));
            }
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Token revoked successfully.", statusCode: (int)HttpStatusCode.OK));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAccount(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "User not found" }, statusCode: (int)HttpStatusCode.NotFound));

            var previousStatus = user.IsActive;
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) 
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            try
            {
                var statusUpdated = await _systemUserRepository.UpdateUserStatus(id, false, cancellationToken);
                if (!statusUpdated)
                {
                    await RestoreIdentityUserStatus(user, previousStatus);
                    return StatusCode(
                        (int)HttpStatusCode.InternalServerError,
                        ApiResponse<object>.ErrorResponse(
                            new List<string> { "Unable to update the system user status" },
                            statusCode: (int)HttpStatusCode.InternalServerError));
                }
            }
            catch
            {
                await RestoreIdentityUserStatus(user, previousStatus);
                throw;
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Account deactivated successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> ActivateAccount(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "User not found" }, statusCode: (int)HttpStatusCode.NotFound));

            var previousStatus = user.IsActive;
            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Errors.Select(e => e.Description).ToList(), statusCode: (int)HttpStatusCode.BadRequest));

            try
            {
                var statusUpdated = await _systemUserRepository.UpdateUserStatus(id, true, cancellationToken);
                if (!statusUpdated)
                {
                    await RestoreIdentityUserStatus(user, previousStatus);
                    return StatusCode(
                        (int)HttpStatusCode.InternalServerError,
                        ApiResponse<object>.ErrorResponse(
                            new List<string> { "Unable to update the system user status" },
                            statusCode: (int)HttpStatusCode.InternalServerError));
                }
            }
            catch
            {
                await RestoreIdentityUserStatus(user, previousStatus);
                throw;
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Account activated successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return NotFound(ApiResponse<object>.ErrorResponse(new List<string> { "User not found" }, statusCode: (int)HttpStatusCode.NotFound));

            var systemUserDeleted = await _systemUserRepository.DeleteUser(id, cancellationToken);
            if (!systemUserDeleted)
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    ApiResponse<object>.ErrorResponse(
                        new List<string> { "Unable to delete the system user" },
                        statusCode: (int)HttpStatusCode.InternalServerError));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var systemUserRestored = await _systemUserRepository.CreateUser(
                    CreateSystemUser(user), cancellationToken);
                var errors = result.Errors.Select(e => e.Description).ToList();

                if (!systemUserRestored)
                    errors.Add("Failed to restore the system user after Identity deletion failed");

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    errors, statusCode: (int)HttpStatusCode.BadRequest));
            }

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

        private async Task RollbackRegistration(AppUser user, string? role = null)
        {
            var rollbackErrors = new List<string>();

            if (role != null)
            {
                var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, role);
                if (!removeRoleResult.Succeeded)
                    rollbackErrors.AddRange(removeRoleResult.Errors.Select(e => e.Description));
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                rollbackErrors.AddRange(deleteResult.Errors.Select(e => e.Description));

            if (rollbackErrors.Count > 0)
                throw new InvalidOperationException(
                    $"Registration rollback failed: {string.Join("; ", rollbackErrors)}");
        }

        private async Task RestoreIdentityUserStatus(AppUser user, bool previousStatus)
        {
            user.IsActive = previousStatus;
            var restoreResult = await _userManager.UpdateAsync(user);

            if (!restoreResult.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to restore the Identity user status: {string.Join("; ", restoreResult.Errors.Select(e => e.Description))}");
        }

        private static SystemUser CreateSystemUser(AppUser user)
        {
            return new SystemUser
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Mobile = user.MobileNumber ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                IdentityUserId = user.Id
            };
        }
    }
}
