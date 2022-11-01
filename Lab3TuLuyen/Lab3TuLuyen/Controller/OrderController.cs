using Lab3TuLuyen.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab3TuLuyen.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public PRN231DBContext _context;
        //key declaration
        private readonly IConfiguration _configuration;
        public OrderController(PRN231DBContext _context, IConfiguration configuration)
        {
            this._context = _context;
            _configuration = configuration;
        }

        // Admin autho
        // Get all order
        // http://localhost:5000/api/Order/GetOrders
        [Authorize()]
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllOrders()
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
        // Get all order by employee id 
        // http://localhost:5000/api/Order/GetOrdersByEmpID/1
        [Authorize()]
        [HttpGet("[action]/{empId}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetOrdersByEmpID(int empId)
        {
            try
            {
                var O = await _context.Orders
                    .Where(x => x.EmployeeId == empId)
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
        [Authorize()]
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
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var O = await _context.Employees
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
        [HttpGet("[action]/{orderId}")]
        public async Task<IActionResult> GetOrderdetail(int orderId)
        {
            try
            {
                var O = await _context.OrderDetails
                    .Where(o => o.OrderId == orderId)
                    .Include(x => x.Product)
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

        [AllowAnonymous]
        [HttpPost("login")]
        //  cust1@gmail.com - 123 - 2
        // admin1@fpt.edu.vn - 123 - 1
        public async Task<IActionResult> Post(Account account)
        {
            if (account.Email != null && account.Password != null)
            {
                var acc = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email && x.Password == account.Password);
                if (acc != null)
                {
                    // Create Claim details
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, acc.Role.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(50),
                            signingCredentials: signIn
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest("Fail here");
            }
        }

        [HttpGet("[action]/{from}/{to}")]
        public async Task<ActionResult<IEnumerable<Customer>>> FilterDate(DateTime from, DateTime to)
        {
            try
            {
                var O = await _context.Orders
                    .Where(o => o.OrderDate >= from && o.OrderDate <= to)
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
