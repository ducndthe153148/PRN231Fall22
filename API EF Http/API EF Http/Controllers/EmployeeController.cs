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
        public EmployeeController(PRN231DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var E = dBContext.Employees.Include(d => d.Department).ToList();
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
        public IActionResult GetById([FromRoute]int id)
        {
            var E = dBContext.Employees.Where(e => e.EmployeeId == id)
                .FirstOrDefault();
            if (E != null)
            {
                return Ok(E);
            } else
            {
                return BadRequest();
            }
        }

        [HttpGet("[action]/{lname}")]
        public IActionResult GetByName([FromRoute] string lname)
        {
            var E = dBContext.Employees.Where(e => e.LastName.Contains(lname)).FirstOrDefault();
            if (E != null)
            {
                return Ok(E);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("[action]/{lname}/{fname}")]
        public IActionResult GetByFLName([FromRoute] string lname, string fname)
        {
            var E = dBContext.Employees.Where(e => e.LastName.Contains(lname) && e.FirstName.Contains(fname)).ToList();
            if (E.Count != 0)
            {
                return Ok(E);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost()]
        public IActionResult Post(Employee E)
        {
            if (ModelState.IsValid)
            {
                dBContext.Add<Employee>(E);
                dBContext.SaveChanges();
                return CreatedAtAction("Get", new {id=E.EmployeeId}, E);
            } else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPut()]
        public IActionResult Put([FromBody]Employee E)
        {
            var employee = dBContext.Employees.Where(e => e.EmployeeId == E.EmployeeId).AsNoTracking().FirstOrDefault();
            if(employee != null)
            {
                if (ModelState.IsValid)
                {
                    dBContext.Update(E);
                    dBContext.SaveChanges();
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
        public IActionResult Delete(int id)
        {
            var E = dBContext.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();
            var O = dBContext.Orders.Where(e => e.EmployeeId == id).FirstOrDefault();
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
                dBContext.SaveChanges();
                return Ok("Delete xong");
            }
            
        }
    }
}
