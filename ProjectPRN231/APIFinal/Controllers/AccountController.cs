using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using APIFinal.DataAccess;
using APIFinal.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PRN231DBContext _context;
        //key declaration
        private readonly IConfiguration _configuration;
        public AccountController(PRN231DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Add authorize later 
        //http://localhost:5000/api/Account/GetPersonalAccount/1
        //[Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetPersonalAccount(int id)
        {
            var A = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);
            var C = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == A.CustomerId);
            // New DTO To Show data 
            AccCusDTO accCusDTO = new AccCusDTO {
                AccountId = A.AccountId,
                Email = A.Email,
                Password = A.Password,
                Role = A.Role,
                CustomerId = A.CustomerId,
                CompanyName = C.CompanyName,
                ContactName = C.ContactName,
                ContactTitle = C.ContactTitle,
                Address = C.Address
            };
            if (accCusDTO == null)
            {
                return NoContent();
            }
            return Ok(accCusDTO);
        }

        [HttpGet("[action]/{email}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountByEmail(string email)
        {
            var A = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            var C = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == A.CustomerId);
            // New DTO To Show data 
            AccCusDTO accCusDTO = new AccCusDTO
            {
                AccountId = A.AccountId,
                Email = A.Email,
                Password = A.Password,
                Role = A.Role,
                CustomerId = A.CustomerId,
                CompanyName = C.CompanyName,
                ContactName = C.ContactName,
                ContactTitle = C.ContactTitle,
                Address = C.Address
            };
            if (accCusDTO == null)
            {
                return NoContent();
            }
            return Ok(accCusDTO);
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
                return BadRequest();
            }
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        // http://localhost:5000/api/Account/Register/
        public async Task<IActionResult> Register([FromBody] AccCusDTO account)
        {
            try
            {
                var customerId = RandomString(5);
                if (account != null && account.Email != null && account.Password != null)
                {
                    _context.Add<Customer>(new Customer
                    {
                        CustomerId = customerId,
                        CompanyName = account.CompanyName,
                        ContactName = account.ContactName,
                        ContactTitle = account.ContactTitle,
                        Address = account.Address
                    });
                    _context.Add<Account>(new Account
                    {
                        Email = account.Email,
                        Password = account.Password,
                        Role = account.Role,
                        CustomerId = customerId
                    });

                    _context.SaveChanges();
                    return Ok(account);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Fail at running");
            }
        }
        private string RandomString(int length)
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
