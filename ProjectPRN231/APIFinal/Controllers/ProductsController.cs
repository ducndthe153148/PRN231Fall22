using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFinal.DataAccess;
using System.Numerics;
using System.Security.Claims;
using APIFinal.Service;
using APIFinal.DTO;
using System.Linq.Expressions;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public PRN231DBContext _context;
        ProductService service = new ProductService();
        public ProductsController(PRN231DBContext _context)
        {
            this._context = _context;
        }
        // http://localhost:5000/api/Products/getFourHot
        [HttpGet("getFourHot")]
        public async Task<IActionResult> GetHotProducts()
        {
            try
            {
                List<Product> GetTopDiscountProducts = new List<Product>();
                var GetTopDiscountOrderDetails = await _context.OrderDetails.OrderByDescending(x => x.Discount).Take(4).ToListAsync();
                foreach (var order in GetTopDiscountOrderDetails)
                {
                    Product p = service.GetProductById(order.ProductId);
                    GetTopDiscountProducts.Add(service.GetProductById(order.ProductId));
                }
                return Ok(GetTopDiscountProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getFourBestSale")]
        public async Task<IActionResult> GetBestSaleProducts()
        {
            try
            {
                List<Product> GetBestSaleProducts = new List<Product>();
                var GetBestSaleOrderDetails = await _context.OrderDetails
                    .GroupBy(x => x.ProductId)
                    .Select(c => new CountDTO
                    {
                        ProductId = c.FirstOrDefault().ProductId,
                        Count = c.Count()
                    })
                    .OrderByDescending(o => o.Count)
                    .Take(4).ToListAsync();
                foreach (var order in GetBestSaleOrderDetails)
                {
                    Product p = service.GetProductById(order.ProductId);
                    GetBestSaleProducts.Add(p);
                }
                return Ok(GetBestSaleProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // http://localhost:5000/api/Products/getFourNew
        [HttpGet("getFourNew")]
        public async Task<IActionResult> GetNewestProducts()
        {
            List<Product> list = await _context.Products.OrderByDescending(x => x.ProductId).Take(4).ToListAsync();
            return Ok(list);
        }
        [HttpGet("getAllProduct")]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
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

        // Get all product by category id 
        [HttpGet("[action]/{productId}")]
        public async Task<ActionResult<Product>> GetByProdId(int productId)
        {
            try
            {
                var E = await _context.Products.OrderByDescending(p => p.ProductId)
                .Where(x => x.ProductId == productId)
                .Include(c => c.Category)
                .FirstOrDefaultAsync();
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

        // Get all product by category id 
        [HttpGet("filterCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> Filter(int categoryId)
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
        public async Task<ActionResult<IEnumerable<Product>>> Search(string searchString)
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

        // API search and filter by categoryid
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchFilter(int categoryId = 0, string searchString = "")
        {
            try
            {
                if(categoryId == 0)
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
                } else if (searchString == "")
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
                } else
                {
                    var E = await _context.Products.OrderByDescending(p => p.ProductId)
                .Where(x => x.ProductName.Contains(searchString) && x.CategoryId == categoryId)
                .Include(c => c.Category)
                .ToListAsync();
                    if (E == null)
                    {
                        return NoContent();
                    }
                    return Ok(E);
                }
                
            }
            catch (Exception E)
            {
                return BadRequest(E);
            }
        }

        //[Authorize("1")]  
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

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(ProductEdit product)
        {
            //var product = await _context.Products.Where(e => e.ProductId == E.ProductId)
            //    .AsNoTracking().FirstOrDefaultAsync();
            var edited = await _context.Products.SingleOrDefaultAsync(o => o.ProductId == product.ProductId);
            if (edited == null)
            {
                return BadRequest();
            }
            try
            {
                edited.ProductName = product.ProductName;
                edited.CategoryId = product.CategoryId;
                edited.QuantityPerUnit = product.QuantityPerUnit;
                edited.UnitPrice = product.UnitPrice;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        //[Authorize("1")]
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
        // Authorize admin and seller 
        //[Authorize("1")]
        [HttpGet("TestRole1")]
        public IActionResult SellersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.Email}, you are a {currentUser.Role}");
        }
        [HttpGet("GetCurrentUser")]
        public Account GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new Account
                {
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Role = Int16.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value)
                };
            }
            return null;
        }
    }
}
