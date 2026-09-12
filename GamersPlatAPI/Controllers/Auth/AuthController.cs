using Bussiness;
using Data.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("register")]
        [EnableRateLimiting("AuthLimiter")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid registration data");

            if (UserBLL.ExistsByPhone(request.PhoneNumber))
                return Conflict("Phone already in use");

            var user = new Data.User
            {
                FullName = request.Name ?? string.Empty,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = HashPassword(request.Password),
                CityId = request.CityId,
                CreatedAt = DateTime.UtcNow,
                EmailVerified = false,
                PhoneVerified = false
            };

            var bll = new UserBLL(user);
            if (!bll.Add())
                return StatusCode(500, "Unable to create user");

            // assign role if provided
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                using var db = new GamersPlatDbContext();
                var role = db.Roles.FirstOrDefault(r => r.RoleName == request.Role);
                if (role != null)
                {
                    var ur = new Data.UserRole { UserId = bll._userID, RoleId = role.RoleId };
                    var urBll = new UserRoleBLL(ur);
                    urBll.Add();
                }
            }

            return Created("", new { UserId = bll._userID });
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Phone number and password are required");

            using var db = new GamersPlatDbContext();
            var user = db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null)
                return NotFound("User not found");

            var hashed = HashPassword(request.Password);
            if (hashed != user.PasswordHash)
                return Unauthorized("Invalid credentials");

            // build role claims
            var roleNames = user.UserRoles?.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

            var token = GenerateJwtToken(user.UserId, roleNames);

            // create refresh token record
            var refresh = new Data.RefreshToken
            {
                TokenHash = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.UserId
            };
            var rtBll = new RefreshTokenBLL(refresh);
            rtBll.Add();

            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1),
                Path = "/"
            });

            var resp = new LoginResponse
            {
                UserId = user.UserId,
                Roles = roleNames,
                Token = token,
                RefreshToken = refresh.TokenHash
            };

            return Ok(resp);
        }

        [HttpPost("Refresh")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest();

            var existing = await RefreshTokenBLL.GetByToken(request.RefreshToken);
            if (existing == null)
                return Unauthorized();

            var token = GenerateJwtToken(existing.UserId);
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest();

            var existing = await RefreshTokenBLL.GetByToken(request.RefreshToken);
            if (existing == null) return NotFound();

            var rtBll = new RefreshTokenBLL();
            var deleted = rtBll.Delete(existing.TokenId);
            // remove cookie
            Response.Cookies.Delete("accessToken");
            if (!deleted) return StatusCode(500, new { message = "Failed to revoke refresh token" });
            return NoContent();
        }

        private string GenerateJwtToken(int userId, List<string>? roles = null)
        {
            var secret = _config["GamersPlat_JKey"] ?? throw new Exception("JWT secret not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            if (roles != null)
            {
                foreach (var r in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            var token = new JwtSecurityToken(
                issuer: "GamersPlat",
                audience: "GamersPlatCustomers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("request-verify-email")]
        public IActionResult RequestVerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (request == null || (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber)))
                return BadRequest();

            using var db = new GamersPlatDbContext();
            var user = !string.IsNullOrWhiteSpace(request.Email)
                ? db.Users.FirstOrDefault(u => u.Email == request.Email)
                : db.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null) return NotFound();

            var token = new Data.EmailVerificationToken
            {
                UserId = user.UserId,
                TokenHash = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow
            };

            var bll = new EmailVerificationTokenBLL(token);
            bll.Add();

            // In production send email. Return token for testing.
            return Ok(new { Token = token.TokenHash });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] TokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token)) return BadRequest();

            var token = await EmailVerificationTokenBLL.GetByToken(request.Token);
            if (token == null) return NotFound();
            if (token.ExpiresAt < DateTime.UtcNow) return BadRequest("Token expired");

            var userTask = UserBLL.GetByID(token.UserId);
            userTask.Wait();
            var user = userTask.Result;
            if (user == null) return NotFound();

            user.EmailVerified = true;
            var userBll = new UserBLL(user);
            userBll.Update();

            var evBll = new EmailVerificationTokenBLL();
            evBll.Delete(token.TokenIdId);

            return Ok(new { message = "Email verified" });
        }

        [HttpPost("request-reset-password")]
        public IActionResult RequestResetPassword([FromBody] RequestResetRequest request)
        {
            if (request == null || (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber)))
                return BadRequest();

            using var db = new GamersPlatDbContext();
            var user = !string.IsNullOrWhiteSpace(request.Email)
                ? db.Users.FirstOrDefault(u => u.Email == request.Email)
                : db.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null) return NotFound();

            var token = new Data.PasswordResetToken
            {
                UserId = user.UserId,
                TokenHash = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

            var bll = new PasswordResetTokenBLL(token);
            bll.Add();

            // Return token for testing; in production send via SMS/email
            return Ok(new { Token = token.TokenHash });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest();

            var token = await PasswordResetTokenBLL.GetByToken(request.Token);
            if (token == null) return NotFound();
            if (token.ExpiresAt < DateTime.UtcNow) return BadRequest("Token expired");

            var userTask = UserBLL.GetByID(token.UserId);
            userTask.Wait();
            var user = userTask.Result;
            if (user == null) return NotFound();

            user.PasswordHash = HashPassword(request.NewPassword);
            var userBll = new UserBLL(user);
            userBll.Update();

            var prBll = new PasswordResetTokenBLL();
            prBll.Delete(token.TokenId);

            return Ok(new { message = "Password reset" });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId)) return Unauthorized();

            var user = await UserBLL.GetByID(userId);
            if (user == null) return NotFound();

            if (HashPassword(request.OldPassword) != user.PasswordHash) return Unauthorized("Invalid old password");

            user.PasswordHash = HashPassword(request.NewPassword);
            var bll = new UserBLL(user);
            if (!bll.Update()) return StatusCode(500);

            return NoContent();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        // DTOs
        public record RegisterRequest(string? Name, string? Email, string PhoneNumber, string Password, string? Role, short CityId);
        public record LoginRequest(string PhoneNumber, string Password);
        public record LoginResponse
        {
            public int UserId { get; init; }
            public List<string>? Roles { get; init; }
            public string Token { get; init; } = null!;
            public string RefreshToken { get; init; } = null!;
        }

        public record RefreshRequest(string RefreshToken);
        public record VerifyEmailRequest(string? Email, string? PhoneNumber);
        public record TokenRequest(string Token);
        public record RequestResetRequest(string? Email, string? PhoneNumber);
        public record ResetPasswordRequest(string Token, string NewPassword);
        public record ChangePasswordRequest(string OldPassword, string NewPassword);
    }
}
