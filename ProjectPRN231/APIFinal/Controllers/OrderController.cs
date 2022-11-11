using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFinal.DataAccess;

namespace APIFinal.Controllers
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
        //[Authorize]
        [HttpGet("GetOrdersByCustomerId/{customerId}")]
        public async Task<ActionResult<Customer>> GetById(string customerId)
        {
            try
            {
                var O = await _context.Customers
                    .Include(o => o.Orders.OrderByDescending(o => o.OrderDate))
                    .ThenInclude(a => a.OrderDetails)
                    .ThenInclude(p => p.Product)
                    .SingleOrDefaultAsync(i => i.CustomerId == customerId);
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

        [HttpGet("[action]/{orderId}")]
        public async Task<ActionResult<Customer>> GetOrderByOrderId(int orderId)
        {
            try
            {
                var O = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(p => p.Product)
                    .SingleOrDefaultAsync(i => i.OrderId == orderId);
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
        [Authorize()]
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetOrders()
        {
            try
            {
                var O = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Include(o => o.Employee)
                    .Include(o => o.Customer)
                    .Take(20)
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

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }
        // Admin autho
        // Get order between two OrderDate
        // http://localhost:5000/api/Order/Between
        // Lam sau
        //[Authorize("1")]
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

        [HttpGet("[action]/{from}/{to}")]
        public async Task<ActionResult<IEnumerable<Customer>>> FilterDate(DateTime from, DateTime to)
        {
            try
            {
                var O = await _context.Orders
                    .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                    .Include(o => o.Employee)
                    .Include(o => o.Customer)
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

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(OrderAdd order)
        {
            try
            {
                var cust = await _context.Customers.SingleOrDefaultAsync(o => o.CustomerId == order.CustomerId);
                Order newOrder = new Order
                {
                    CustomerId = order.CustomerId,
                    EmployeeId = 3,
                    OrderDate = DateTime.Now,
                    RequiredDate = order.RequiredDate,
                    ShipName = cust.ContactName,
                    ShipAddress = cust.Address,
                    ShippedDate = order.ShippedDate,
                    Freight = order.Freight
                };
                _context.Orders.Add(newOrder);
                _context.SaveChanges();
                await _context.SaveChangesAsync();

                var latestId = _context.Orders.OrderBy(o => o.OrderId).LastOrDefault();

                return Ok(latestId.OrderId);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpPut("[action]/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var O = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (O == null)
                {
                    return NoContent();
                }
                O.RequiredDate = null;
                _context.Update(O);
                await _context.SaveChangesAsync();
                return Ok(O);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetYear()
        {
            List<int> listYear ;
            try
            {
                listYear = _context.Orders
                    //.GroupBy(x => x.OrderDate)
                    .Select(x => x.OrderDate.Value.Year).Distinct().ToList();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(listYear);
        }
    }
}
