using API_EF_Http.DataAccess;
using API_EF_Http.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace API_EF_Http.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        public PRN231DBContext dBContext;
        public CustomersController(PRN231DBContext dBContext)
        {
			this.dBContext = dBContext;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> List()
        {
            var E = await dBContext.Customers.ToListAsync();
            if (E == null)
            {
                return NoContent();
            }
            var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
            var mapper = config.CreateMapper();
            List<CustomerDTO> result = E.Select(e => mapper.Map<Customer, CustomerDTO>(e)).ToList();
            return Ok(result);
        }
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> List(string id)
        {
            var E = await dBContext.Customers.FindAsync(id);
            if (E == null)
            {
                return BadRequest();
            }
            return Ok(E);
        }
        [HttpGet("get-orders/{id}")]
        public async Task<IActionResult> GetOrders(string id)
        {
            var orders = await dBContext.Orders
                .Include(c => c.Customer)
                .Join(dBContext.OrderDetails, x => x.OrderId, y => y.OrderId, (x, y) => new
                {
                    OrderId = x.OrderId,
                    OrderDate = x.OrderDate, 
                    CustomerName = x.Customer.ContactName,
                    ProductName = y.Product.ProductName,
                    Price = y.UnitPrice,
                    Quantity = y.Quantity
                }).ToListAsync();
            return Ok(orders);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(Customer E)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var C = await dBContext.Customers.FindAsync(E.CustomerId);
                if (C != null)
                    return NoContent();

                await dBContext.AddAsync<Customer>(E);
                await dBContext.SaveChangesAsync();
                return Ok(E);
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            var C = await dBContext.Customers.FindAsync(id);
            if (C == null) { return NotFound(); }
            else
            {
                var test = await dBContext.Customers.Include(o => o.Orders).SingleOrDefaultAsync(c => c.CustomerId == id);
                var O = await dBContext.Orders.Where(e => e.CustomerId == id).FirstOrDefaultAsync();
                bool check = false;
                if (test.Orders.Count > 0)
                {
                    check = true;
                }
                if (check)
                {
                    return NoContent();
                }
                    dBContext.Remove<Customer>(C);
                    await dBContext.SaveChangesAsync();
                    return Ok(C);
                
            }
        }


    }
}
