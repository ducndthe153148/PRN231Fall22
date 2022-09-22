using API_EF_Http.DataAccess;
using API_EF_Http.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_EF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        public PRN231DBContext dBContext;
        public IMapper _mapper;
        public EmployeeController(PRN231DBContext dBContext, IMapper mapper)
        {
            this.dBContext = dBContext;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var E = await dBContext.Employees.Include(d => d.Department).ToListAsync();
            if(E == null)
            {
                return NoContent();
            }
            var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
            var mapper = config.CreateMapper();
            List<EmployeeDTO> result = E.Select(e => mapper.Map<Employee, EmployeeDTO>(e)).ToList();
            return Ok(result);
        }   
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var E = await dBContext.Employees.Where(e => e.EmployeeId == id)
                .Include(d => d.Department)
                .FirstOrDefaultAsync();
            if (E == null)
            {
                return BadRequest();
            }
            var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
            var mapper = config.CreateMapper();
            EmployeeDTO result = _mapper.Map<Employee, EmployeeDTO>(E);
            return Ok(result);
        }

        [HttpGet("[action]/{lname}")]
        public async Task<IActionResult> GetByName([FromRoute] string lname)
        {
            var E = await dBContext.Employees.Where(e => e.LastName.Contains(lname))
                .Include(d => d.Department)
                .FirstOrDefaultAsync();
            if (E != null)
            {
                return BadRequest();
            }
            var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
            var mapper = config.CreateMapper();
            EmployeeDTO result = mapper.Map<Employee, EmployeeDTO>(E);
            return Ok(result);
        }

        [HttpGet("[action]/{lname}/{fname}")]
        public async Task<IActionResult> GetByFLName([FromRoute] string lname, string fname)
        {
            var E = await dBContext.Employees.Where(e => e.LastName.Contains(lname) && e.FirstName.Contains(fname))
                .Include(d => d.Department)
                .ToListAsync();
            if (E.Count == null)
            {
                return BadRequest();
            }
            var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
            var mapper = config.CreateMapper();
            List<EmployeeDTO> result = E.Select(e => mapper.Map<Employee, EmployeeDTO>(e)).ToList();
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Post(EmployeeDTO E)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
                var mapper = config.CreateMapper();
                var result = mapper.Map<Employee>(E);

                await dBContext.AddAsync<Employee>(result);
                await dBContext.SaveChangesAsync();
                return CreatedAtAction("Get", new { id = E.EmployeeId }, E);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] EmployeeDTO E)
        {
            var employee = await dBContext.Employees.Where(e => e.EmployeeId == E.EmployeeId).AsNoTracking().FirstOrDefaultAsync();
            if(employee != null)
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(config => config.AddProfile(new MappingDTO()));
                    var mapper = config.CreateMapper();
                    var result = mapper.Map<Employee>(E);

                    dBContext.Update(result);
                    dBContext.SaveChangesAsync();
                    return AcceptedAtAction("Put", new { id = E.EmployeeId }, E);
                } else
                {
                    return BadRequest(E.EmployeeId);
                }
               
            } else
            {
                return NotFound();
            }
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var E = await dBContext.Employees.Where(e => e.EmployeeId == id).FirstOrDefaultAsync();
            var O = await dBContext.Orders.Where(e => e.EmployeeId == id).FirstOrDefaultAsync();
            bool check = false;
            if(O != null)
            {
                check = true;
            }
            if(check)
            {
                return BadRequest("Ga");
            } else
            {
                dBContext.Remove<Employee>(E);
                await dBContext.SaveChangesAsync();
                return Ok("Delete xong");
            }
            
        }
    }
}
