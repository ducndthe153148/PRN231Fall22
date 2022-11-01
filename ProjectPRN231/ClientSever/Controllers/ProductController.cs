using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientSever.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        readonly string apiBaseAddress = "http://localhost:5000/api/";
        public async Task<ActionResult> ManageProduct()
        {
            IEnumerable<Product> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("Products/getAllProduct");

                if (result.IsSuccessStatusCode)
                {
                    products = await result.Content.ReadAsAsync<IList<Product>>();
                    ViewData["products"] = products;
                }
                else
                {
                    products = Enumerable.Empty<Product>();
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            return View();
        }

    }
}
