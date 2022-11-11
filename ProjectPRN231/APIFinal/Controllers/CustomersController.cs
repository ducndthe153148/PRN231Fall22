using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFinal.DataAccess;
using System.Text;

namespace APIFinal.Controllers
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
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(CustomerEdit customer)
        {
            var edited = await _context.Customers.SingleOrDefaultAsync(o => o.CustomerId == customer.CustomerId);
            //var account = await _context.Accounts.SingleOrDefaultAsync(a => a.CustomerId == customer.CustomerId);
            if (edited == null)
            {
                return BadRequest();
            }
            edited.Address = customer.Address;
            edited.CompanyName = customer.CompanyName;
            edited.ContactName = customer.ContactName;
            edited.ContactTitle = customer.ContactTitle;
            //account.Email = customer.Email;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
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
        [HttpPost]
        public async Task<ActionResult<string>> CreateGuess(GuessAdd guess)
        {
            try
            {
                string id = RandomCustID(5);
                _context.Customers.Add(new Customer
                {
                    CustomerId = id,
                    CompanyName = guess.CompanyName,
                    ContactName = guess.ContactName,
                    Address = guess.Address,
                    ContactTitle = guess.ContactTitle,
                });
                await _context.SaveChangesAsync();


                return Ok(id);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<NewCustomer>> GetNewCustomer()
        {
            NewCustomer customer = new NewCustomer();
            try
            {
                // Total customer va customer theo thang
                var total = _context.Customers.Count();
                // Get all order by customer in 30day gan nhat
                var orders = _context.Orders
                    .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30));

                // Loai order trung customerId
                var newCus = orders.Select(x => x.CustomerId).Distinct().Count();

                customer.Total = total;
                customer.New = newCus;
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(customer);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetTotalCustomer()
        {
            int totalCustomer = 0;
            try
            {
                totalCustomer = _context.Customers.Count();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(totalCustomer);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetTotalGuest()
        {
            int guestCount = 0;
            try
            {
                // Count total distinct CustomerId in Orders
                var totalCus = _context.Orders
                    .Select(x => x.CustomerId).Distinct().Count();
                // Count total Account 
                var totalAccount = _context.Accounts
                    .Select(x => x.CustomerId).Distinct().Count();
                // Sub => Guest 
                guestCount = totalCus - totalAccount;
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(guestCount);
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<decimal>> GetTotalOrders()
        {
            decimal totalOrder = 0;
            try
            {
                // count total order in order detail
                totalOrder = (decimal) _context.Orders
                    .Select(x => x.Freight)
                    .Sum();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(Math.Round(totalOrder));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<decimal>> GetWeeklySale()
        {
            decimal totalOrder = 0;
            decimal weeklySale = 0;
            try
            {
                // Get order detail from orderId in 7 day before 
                var orders = _context.Orders
                     .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))
                     .ToList();
                weeklySale = (decimal) orders.Select(x => x.Freight).Sum();
                // count total of each order in 7 day
                var eachOrder = orders.Select(x => x.OrderId);
                foreach (var item in eachOrder)
                {
                    var order = await _context.OrderDetails.Where(o => o.OrderId == item).ToListAsync();
                    totalOrder += order.Select(x => x.UnitPrice).Sum();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok(Math.Round(weeklySale));
        }
        private string RandomCustID(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

    }
}
