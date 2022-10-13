using API_EF_Http.DataAccess;
using API_EF_Http.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_EF_Http.Controllers
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

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Account>>> Get()
        {
            var E = await _context.Accounts.ToListAsync();
            if (E == null)
            {
                return NoContent();
            }
            return Ok(E);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount([FromBody]AccountDTO account)
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

        [HttpPost("login")]
        public async Task<IActionResult> Post(Account account)
        {
            if (account != null && account.Email != null && account.Password != null)
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
                        new Claim("AccountId", acc.AccountId.ToString()),
                        new Claim("Password", acc.Password),
                        new Claim("Email", acc.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
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
    }
}
