using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN231.DataAccess;
using System.Text;

namespace ProjectPRN231.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public PRN231DBContext _context;
        public CategoriesController(PRN231DBContext context)
        {
            _context = context;
        }
        // http://localhost:5000/api/Categories/ListCategory/
        [HttpGet("[action]")]
        public IActionResult ListCategory()
        {
            try
            {
                var E = _context.Categories.ToList();
                if (E == null)
                {
                    return NoContent();
                }

                return Ok(E);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(Category E)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    return BadRequest(ModelState);
                string str = "Hello World";
                byte[] barr = Encoding.ASCII.GetBytes(str);
                E.Picture = barr;
                await _context.AddAsync<Category>(E);
                await _context.SaveChangesAsync();
                return Ok(E);
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
    }
}
