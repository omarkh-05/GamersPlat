using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassController : ControllerBase
    {
        [HttpPost("generate-hash")]
        public IActionResult GenerateHash([FromBody] DTO_Pass pass)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(pass.Pass);

            return Ok(new { Hash = hash });
        }
    }
    public class DTO_Pass
    {
        public string Pass { get; set; }
    }
}
