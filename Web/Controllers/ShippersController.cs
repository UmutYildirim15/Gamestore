using Microsoft.AspNetCore.Mvc;
using Northwind.Business.Services.Abstract;

namespace Web.Controllers
{
    [Route("shippers")]
    [ApiController]
    public class ShippersController(IMongoShipperService mongoShipperService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetShippers()
        {
            var shippers = await mongoShipperService.GetShippersAsync();
            return Ok(shippers);
        }
    }
}