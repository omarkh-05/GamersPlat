using Bussiness;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Owner;

namespace GamersPlatAPI.Controllers.Owner
{
    [ApiController]
    [Route("api/owner/[controller]")]
    [Authorize(Roles = "Owner")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourcesTypeService _resources;
        private readonly ICenterService _center;

        public ResourcesController(IResourcesTypeService resources, ICenterService center)
        {
            _resources = resources;
            _center = center;
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] DTO_CreateResourcesType request)
        {
            var idClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out var uid))
                return Unauthorized();

            var center = await _center.GetByID(request.CenterId);

            if (center == null)
                return NotFound();

            // Ensure the owner owns the center
            if (center.OwnerUserId != uid)
                return Forbid();

            var rt = new Data.ResourcesType
            {
                CenterId = request.CenterId,
                DeviceId = request.DeviceId,
                HourlyPrice = request.HourlyPrice,
                TotalQuantity = request.TotalQuantity,
                RoomType = request.RoomType,
                IsActive = request.IsActive
            };

            if (!await _resources.Add(rt))
                return StatusCode(500);

            return CreatedAtAction(
                nameof(GetById),
                new { id = _resources.LastId },
                rt);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rt = await _resources.GetByID(id);
            if (rt == null) return NotFound();
            return Ok(rt);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] DTO_UpdateResourcesType request)
        {
            var idClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out var uid))
                return Unauthorized();

            var existing = await _resources.GetByID(id);

            if (existing == null)
                return NotFound();

            // Ensure the owner owns the center this resource belongs to
            var center = await _center.GetByID(existing.CenterId);

            if (center == null || center.OwnerUserId != uid)
                return Forbid();

            // Update only the fields the owner is allowed to change
            existing.DeviceId = request.DeviceId;
            existing.HourlyPrice = request.HourlyPrice;
            existing.TotalQuantity = request.TotalQuantity;
            existing.RoomType = request.RoomType;
            existing.IsActive = request.IsActive;

            if (!await _resources.Update(existing))
                return StatusCode(500);

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _resources.GetByID(id);
            if (existing == null) return NotFound();

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var center = await _center.GetByID(existing.CenterId);
            if (center == null || center.OwnerUserId != uid) return Forbid();
            if (!await _resources.Delete(id)) return StatusCode(500);
            return NoContent();
        }
    }
}
