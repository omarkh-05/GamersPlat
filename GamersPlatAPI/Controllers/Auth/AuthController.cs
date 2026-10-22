using Bussiness.Interfaces;
using Data;
using Domain.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController( IAuthService authService)
        {
            _authService = authService;
        }
       
        // ================ User Auth Management ================
        private CookieOptions RefreshCookieOptions() => new()
        {
            HttpOnly = true,
            Secure = true, // in production it must be true
            SameSite = SameSiteMode.None, // in production it must be SameSiteMode.Strict - مهم عند cross-origin (127.0.0.1:5500 → localhost:7018)
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        };

        [HttpPost("register/player")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlayerRegister([FromBody] UserRegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid registration data");

            try
            {
                await _authService.RegisterWithRoleAsync(request, "Player");
                return Ok(new { message = "Player registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Registration failed. Please try again." + ex.Message);
            }
        }

        [HttpPost("register/owner")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OwnerRegister([FromBody] UserRegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid registration data");

            try
            {
                await _authService.RegisterWithRoleAsync(request, "Owner");
                return Ok(new { message = "Owner registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Registration failed. Please try again." + ex.Message);
            }
        }

        [HttpPost("Login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrEmpty(request.PhoneNumber) ||
                string.IsNullOrEmpty(request.Password))
                return BadRequest("Phone number and password are required");

            try
            {
                var result = await _authService.LoginAsync(request);
                Response.Cookies.Append("refreshToken", result.RefreshToken, RefreshCookieOptions());
                
                return Ok(new
                {
                    result.AccessToken,
                    result.UserName,
                    result.UserRole
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }

        [HttpPost("Refresh")]
        [EnableRateLimiting("RefreshLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) ||
                    string.IsNullOrEmpty(refreshToken))
                    throw new UnauthorizedAccessException("Refresh token missing");

                var result = await _authService.RefreshAsync(refreshToken);

                // rotation: الجديد بالكوكي، والقديم لازم يكون انلغى بالـ BLL
                Response.Cookies.Append("refreshToken", result.RefreshToken, RefreshCookieOptions());

                return Ok(new { result.AccessToken, result.UserName, result.UserRole });
            }
            catch (UnauthorizedAccessException ex)
            {
                // إذا فشل، امسح الكوكي الفاسدة
                Response.Cookies.Delete("refreshToken", RefreshCookieOptions());
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    await _authService.LogoutAsync(refreshToken);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Logout failed. Please try again later." + ex.Message);
            }

            Response.Cookies.Delete("refreshToken", RefreshCookieOptions());
            return Ok("Logged out successfully");
        }
        // ================ User Auth Management ================


        // ================ Password Management ================
        [Authorize]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (userId <= 0) return Unauthorized();

                var result = await _authService.ChangePassword(userId, request);

                return StatusCode(result.StatusCode, new
                {
                    success = result.Success,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error changing password: " + ex.Message);
            }
        }

        [HttpPost("reset-password/request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetRequest request)
        {
            try {
            if (request == null || (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber)))
                return BadRequest("Invalid Data");

           string tokenHash = await _authService.RequestResetPassword(request);

                // Return token for testing; in production send via SMS/email
                return Ok(new { Token = tokenHash });
            }catch (Exception ex) {
                return BadRequest("Error in request reset password: " + ex.Message);
            }
        }

        [HttpPost("reset-password/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
                    return BadRequest("Invalid Data");

                if( await _authService.ResetPassword(request))
                    return Ok("Password reset successfully");
                else
                    return StatusCode(500, "Failed to reset password");
            }
            catch (Exception ex)
            {
                return BadRequest("Error in resetting password: " + ex.Message);
            }
        }
        // ================ Password Management ================


        // ================ Verification Management ================
        //[HttpPost("request-verify-email")]
        //public IActionResult RequestVerifyEmail([FromBody] VerifyEmailRequest request)
        //{
        //    if (request == null || (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber)))
        //        return BadRequest();

        //    using var db = new GamersPlatDbContext();
        //    var user = !string.IsNullOrWhiteSpace(request.Email)
        //        ? db.Users.FirstOrDefault(u => u.Email == request.Email)
        //        : db.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

        //    if (user == null) return NotFound();

        //    var token = new Data.EmailVerificationToken
        //    {
        //        UserId = user.UserId,
        //        TokenHash = Guid.NewGuid().ToString(),
        //        ExpiresAt = DateTime.UtcNow.AddHours(24),
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    var bll = new EmailVerificationTokenBLL(token);
        //    bll.Add();

        //    // In production send email. Return token for testing.
        //    return Ok(new { Token = token.TokenHash });
        //}

        //[HttpPost("verify-email")]
        //public async Task<IActionResult> VerifyEmail([FromBody] TokenRequest request)
        //{
        //    if (request == null || string.IsNullOrWhiteSpace(request.Token)) return BadRequest();

        //    var token = await EmailVerificationTokenBLL.GetByToken(request.Token);
        //    if (token == null) return NotFound();
        //    if (token.ExpiresAt < DateTime.UtcNow) return BadRequest("Token expired");

        //    var userTask = UserBLL.GetByID(token.UserId);
        //    userTask.Wait();
        //    var user = userTask.Result;
        //    if (user == null) return NotFound();

        //    user.EmailVerified = true;
        //    var userBll = new UserBLL(user);
        //    userBll.Update();

        //    var evBll = new EmailVerificationTokenBLL();
        //    evBll.Delete(token.TokenIdId);

        //    return Ok(new { message = "Email verified" });
        //}
        // ================ Verification Management ================



        // Helper
        int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId) || userId <= 0)
                return -1;
            return userId;
        }
    }
}
