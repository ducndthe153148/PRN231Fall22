using APIFinal.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Service
{
    public class ProductService
    {
        public PRN231DBContext _context = new PRN231DBContext();
        public Product GetProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            return product;
        }
    }
}
