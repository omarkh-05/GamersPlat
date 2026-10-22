using Bussiness;
using Microsoft.AspNetCore.Mvc;

namespace GamersPlatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        readonly CityBLL _city;
        public CitiesController(CityBLL city)
        {
            _city = city;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _city.GetAll();
            return Ok(cities);
        }

    }
}
