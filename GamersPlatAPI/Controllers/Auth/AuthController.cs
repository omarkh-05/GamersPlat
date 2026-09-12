using Bussiness.Interfaces;
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

        [HttpPost("register/player")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlayerRegister([FromBody] UserRegisterRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid registration data");

                // فهيك ربطناهم مع بعض DI(builder.Services.AddScoped<IAuthService, AuthBLL>();) عملنا program.cs لما اضغط على الفنكشن راح يوديني على الانتر فيس ولكن الانتر فيس هو راح يعرف اي فنكشن مقصود من كلاس الاوث بزنس لانه في ال program.cs 
                await _authService.RegisterWithRoleAsync(request, "Player");

                return Ok(new { message = "Player registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register/owner")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OwnerRegister([FromBody] UserRegisterRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid registration data");

                await _authService.RegisterWithRoleAsync(request, "Owner");

                return Ok(new { message = "Owner registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Password))
                    return BadRequest("Phone number and password are required");

                var result = await _authService.LoginAsync(request);


                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // in production it must be true
                    SameSite = SameSiteMode.None, // in production it must be SameSiteMode.Strict - مهم عند cross-origin (127.0.0.1:5500 → localhost:7018)
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound("Invalid credentials" + ex.Message);
            }
        }

        [HttpPost("Refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                // اقرأ refresh token من HttpOnly cookie
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    throw new UnauthorizedAccessException("Refresh token missing");

                var result = await _authService.RefreshAsync(refreshToken);

                // ضع refresh token الجديد في HttpOnly cookie
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // in production it must be true
                    SameSite = SameSiteMode.None, // in production it must be SameSiteMode.Strict - مهم عند cross-origin (127.0.0.1:5500 → localhost:7018)
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                return Ok(new { AccessToken = result.AccessToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    throw new UnauthorizedAccessException("Refresh token missing");

                if (await _authService.LogoutAsync(refreshToken))
                {
                    Response.Cookies.Delete("refreshToken");
                }
                return Ok("Logged out successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Logout Error Try Again" + ex.Message);
            }
        }

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
                if (request == null || string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                    return BadRequest("Invalid Data");

                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

                if (!await _authService.ChangePassword(userId,request))
                    return StatusCode(500);

                return Ok("Password changed successfully");
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
    }
}
