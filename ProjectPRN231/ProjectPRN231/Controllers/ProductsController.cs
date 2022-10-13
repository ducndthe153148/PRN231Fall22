using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN231.DataAccess;

namespace ProjectPRN231.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public PRN231DBContext _context;
        public ProductsController(PRN231DBContext _context)
        {
            this._context = _context;
        }
        [HttpGet("hot")]
        public async Task<IActionResult> GetHotProducts()
        {
            List<Product> GetTopDiscountProducts = new List<Product>();
            var GetTopDiscountOrderDetails = await _context.OrderDetails.OrderByDescending(x => x.Discount).Take(4).ToListAsync();
            foreach (var order in GetTopDiscountOrderDetails)
            {
                Product p = GetProductById(order.ProductId);
                GetTopDiscountProducts.Add(GetProductById(order.ProductId));
            }
            return Ok(GetTopDiscountProducts);
        }

        public Product GetProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            return product;
        }
        [HttpGet("bestSale")]
        public async Task<IActionResult> GetBestSaleProducts()
        {

            List<Product> GetBestSaleProducts = new List<Product>();
            return Ok(GetBestSaleProducts); 
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewestProducts()
        {
            List<Product> list = await _context.Products.OrderByDescending(x => x.ProductId).Take(4).ToListAsync();
            return Ok(list);
        }
        [HttpGet("getAllProduct")]
        public async Task<ActionResult<IEnumerable<Account>>> Get()
        {
            try
            {
                var E = await _context.Products
                    .OrderByDescending(p => p.ProductId)
                    .Include(c => c.Category)
                    .ToListAsync();
                if (E == null)
                {
                    return NoContent();
                }
                return Ok(E);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }
            
        }
        [HttpGet("filter/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Account>>> Filter(int categoryId)
        {
            try
            {
                var E = await _context.Products.OrderByDescending(p => p.ProductId)
                .Where(x => x.CategoryId == categoryId)
                .Include(c => c.Category)
                .ToListAsync();
                if (E == null)
                {
                    return NoContent();
                }
                return Ok(E);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }
            
        }
        [HttpGet("search/{searchString}")]
        public async Task<ActionResult<IEnumerable<Account>>> Search(string searchString)
        {
            try
            {
                var E = await _context.Products.OrderByDescending(p => p.ProductId)
                .Where(x => x.ProductName.Contains(searchString))
                .Include(c => c.Category)
                .ToListAsync();
                if (E == null)
                {
                    return NoContent();
                }
                return Ok(E);
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Product p)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _context.AddAsync<Product>(p);
                await _context.SaveChangesAsync();
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> Edit(Product E)
        {
            var product = await _context.Products.Where(e => e.ProductId == E.ProductId)
                .AsNoTracking().FirstOrDefaultAsync();
            if (product != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Update(product);
                    _context.SaveChanges();
                    return AcceptedAtAction("edit", new { id = E.ProductId }, E);
                }
                else
                {
                    return BadRequest(E.ProductId);
                }
            }
            else
            {
                return NotFound();
            }

        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var C = await _context.Products.FindAsync(id);
            if (C == null)
            {
                return NotFound();
            }
            else
            {
                var orderDetails = await _context.OrderDetails.Where(x => x.ProductId == C.ProductId).ToListAsync();
                if (orderDetails.Count > 0)
                {
                    return BadRequest();
                } else
                {
                    _context.Remove(C);
                    await _context.SaveChangesAsync();
                    return Ok(C);
                }
            }
        }
    }
}
