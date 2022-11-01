using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN231.DataAccess;

namespace ProjectPRN231.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public PRN231DBContext _context;
        public OrderController(PRN231DBContext _context)
        {
            this._context = _context;
        }

        // User autho
        // Add authorize and authentication 
        // Get all order by id (order by customerid)
        // http://localhost:5000/api/Order/GetOrdersByCustomerId/GODOS
        [Authorize("1")]
        [HttpGet("GetOrdersByCustomerId/{customerId}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetById(string customerId)
        {
            try
            {
                var O = await _context.Orders
                    .Where(c => c.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
                if (O == null)
                {
                    return NoContent();
                }
                return Ok(O);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }

        }

        // Admin autho
        // Get all order
        // http://localhost:5000/api/Order/GetOrders
        [Authorize("2")]
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetOrders()
        {
            try
            {
                var O = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
                if (O == null)
                {
                    return NoContent();
                }
                return Ok(O);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }

        }
        // Admin autho
        // Get order between two OrderDate
        // http://localhost:5000/api/Order/Between
        // Lam sau
        [Authorize("2")]
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetOrdersBetween()
        {
            try
            {
                var O = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
                if (O == null)
                {
                    return NoContent();
                }
                return Ok(O);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }

        }
    }
}
