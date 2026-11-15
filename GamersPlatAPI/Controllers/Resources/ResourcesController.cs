using Bussiness;
using Bussiness.BLL;
using Bussiness.Interfaces;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers.Resources
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        readonly ResourceBLL _resource;

        public ResourcesController(ResourceBLL resource)
        {
            _resource = resource;
        }

        //[HttpPost]
        // public async Task<IActionResult> Create([FromBody] CenterResource rt)
        // {
        //     if (rt == null) return BadRequest();

        //     var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //     if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

        //     // ensure owner owns the center
        //     var center = await _center.GetByID(rt.CenterId);
        //     if (center == null) return NotFound();
        //     if (center.OwnerUserId != uid) return Forbid();
        //     if (!await _centerResources.Add(rt)) return StatusCode(500);
        //     return CreatedAtAction(nameof(GetById), new { id = _centerResources.LastId }, rt);
        // } 

        // [HttpPut("{id:int}")]
        // public async Task<IActionResult> Update(int id, [FromBody] CenterResource rt)
        // {
        //     if (rt == null || rt.ResourcesTypeId != id) return BadRequest();

        //     var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //     if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

        //     var existing = await _centerResources.GetByID(id);
        //     if (existing == null) return NotFound();

        //     var center = await _center.GetByID(existing.CenterId);
        //     if (center == null || center.OwnerUserId != uid) return Forbid();
        //     if (!await _centerResources.Update(rt)) return StatusCode(500);
        //     return NoContent();
        // }

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var existing = await _centerResources.GetByID(id);
        //    if (existing == null) return NotFound();

        //    var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

        //    var center = await _center.GetByID(existing.CenterId);
        //    if (center == null || center.OwnerUserId != uid) return Forbid();
        //    if (!await _centerResources.Delete(id)) return StatusCode(500);
        //    return NoContent();
        //}

        //[HttpGet("{id:int}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var rt = await _centerResources.GetByID(id);
        //    if (rt == null) return NotFound();
        //    return Ok(rt);
        //}


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllResources()
        {
            return await Run(() => _resource.GetAll(), result => Ok(result));
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
