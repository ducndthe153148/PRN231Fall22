using DemoOdata.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace DemoOdata.Controllers
{
    [Route("gadget")]
    [ApiController]
    public class GadgetController : ControllerBase
    {
        private readonly OdataASPNetContext _context;
        public GadgetController(OdataASPNetContext context)
        {
            this._context = context;
        }

        [EnableQuery]
        [HttpGet("Get")]
        public IActionResult Get()
        {
            var E = _context.Gadgets.ToList();
            if (E == null)
            {
                return NoContent();
            }
            
            return Ok(E);
        }

        //[HttpPost("")]
        //public IActionResult AddCountry([FromQuery]Gadget gadget)
        //{
        //    return Ok($"Name = {gadget.ProductName}");
        //}
        //[HttpPost("{name}")]
        //public IActionResult AddCountryRoute([FromRoute]string name, [FromQuery] int id)
        //{
        //    return Ok($"Id = {id} and Name = {name}");
        //}
        //[HttpPost("FromBody")]
        //public IActionResult AddCountryBody([FromBody] string name)
        //{
        //    return Ok($"Name = {name}");
        //}

        //[HttpPost("{id}")]
        //public IActionResult AddCountry([FromRoute] int id, [FromForm] Gadget gadget)
        //{
        //    return Ok($"Name = {gadget.ProductName}");
        //}

        [HttpPost("")]
        public IActionResult AddCountry([FromHeader]string developer)
        {
            return Ok($"developer = {developer}");
        }
    }
}
