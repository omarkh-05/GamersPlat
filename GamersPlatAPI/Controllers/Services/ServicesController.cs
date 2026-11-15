using Bussiness.BLL;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        readonly ServiceBLL _services;
        public ServicesController(ServiceBLL services)
        {
            _services = services;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllServices()
        {
            return await Run(() => _services.GetAll(), result => Ok(result));
        }

        private async Task<IActionResult> Run<T>(Func<Task<T>> action, Func<T, IActionResult> success)
        {
            try
            {
                var result = await action();

                if (result is null)
                    return NotFound();

                return success(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
