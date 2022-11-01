using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN231.DataAccess;

namespace ProjectPRN231.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        public PRN231DBContext _context;
        public CustomersController(PRN231DBContext _context)
        {
            this._context = _context;
        }

        // Find Customer by ID: 
        // http://localhost:5000/api/Customers/GetById/?id
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetById(string id)
        {
            try
            {
                var C = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
                if (C == null)
                {
                    return NoContent();
                }
                return Ok(C);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }

        }
        public Customer GetCustomerById(string id)
        {
            var product = _context.Customers.FirstOrDefault(x => x.CustomerId == id);
            return product;
        }
    }
}
