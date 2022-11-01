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
