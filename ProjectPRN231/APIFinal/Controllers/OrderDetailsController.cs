using APIFinal.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly PRN231DBContext _context;

        public OrderDetailsController(PRN231DBContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> PostOrderDetail(OrderDetailAdd orderDetail)
        {
            try
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = orderDetail.OrderId,
                    Discount = orderDetail.Discount,
                    ProductId = orderDetail.ProductId,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetMonthlyFreight()
        {
            return Ok();
        }
    }
}
