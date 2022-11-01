using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ClientSever.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        List<ClientSever.Models.Product> products;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        readonly string apiBaseAddress = "http://localhost:5000/api/";
        public async Task<ActionResult> Index()
        {
            IEnumerable<Product> hotProduct = null;
            IEnumerable<Product> bestSaleProduct = null;
            IEnumerable<Product> fourNewProduct = null;
            if (HttpContext.Session.GetString("user") != null)
            {
                var user = HttpContext.Session.GetString("user");
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("products/getfourhot");
                var result1 = await client.GetAsync("products/getFourBestSale");
                var result2 = await client.GetAsync("products/getFourNew");

                if (result.IsSuccessStatusCode && result1.IsSuccessStatusCode)
                {
                    hotProduct = await result.Content.ReadAsAsync<IList<Product>>();
                    bestSaleProduct = await result1.Content.ReadAsAsync<IList<Product>>();
                    fourNewProduct = await result2.Content.ReadAsAsync<IList<Product>>();
                }
                else
                {
                    hotProduct = Enumerable.Empty<Product>();
                    bestSaleProduct = Enumerable.Empty<Product>();
                    fourNewProduct = Enumerable.Empty<Product>();
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            ViewData["hotProduct"] = hotProduct;
            ViewData["bestSaleProduct"] = bestSaleProduct;
            ViewData["fourNewProduct"] = fourNewProduct;

            return View();
        }
        public async Task<ActionResult> DetailProduct(int id)
        {
            Product product = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                string url = "Products/GetByProdId/" + id;
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    product = await result.Content.ReadAsAsync<Product>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            ViewData["product"] = product;

            return View();
        }

        public async Task<ActionResult> Category(int id)
        {
            IEnumerable<Product> products = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                string urlUse = "Products/filterCategory/" + id;
                var result = await client.GetAsync(urlUse);

                if (result.IsSuccessStatusCode)
                {
                    products = await result.Content.ReadAsAsync<IList<Product>>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            ViewData["products"] = products;
            return View();
        }
        public async Task<ActionResult> LeftNav()
        {
            return View();
        }

        public async Task<IActionResult> AddProductToCart(int id)
        {
            List<ClientSever.Models.Product> productList = await GetAllProducts();
            var addedProduct = productList.SingleOrDefault(o => o.ProductId == id);
            if (HttpContext.Session.GetString("CartList") != null)
            {
                var option = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                List<Models.CartSession> cart = System.Text.Json.JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

                var check = cart.SingleOrDefault(o => o.ProductId == id);

                //nếu cart đã có sẵn sản phẩm => thêm quantity
                if (check != null)
                {
                    check.Quantity += 1;
                    check.Price += addedProduct.UnitPrice;
                }
                //cart chưa có => add mới 
                else
                {
                    cart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                }
                HttpContext.Session.SetString("CartList", System.Text.Json.JsonSerializer.Serialize(cart));

            }
            else
            {
                List<Models.CartSession> newCart = new List<Models.CartSession>();
                newCart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                HttpContext.Session.SetString("CartList", System.Text.Json.JsonSerializer.Serialize(newCart));
            }


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> BuyNow(int id)
        {
            List<ClientSever.Models.Product> productList = await GetAllProducts();
            var addedProduct = productList.SingleOrDefault(o => o.ProductId == id);
            if (HttpContext.Session.GetString("CartList") != null)
            {
                var option = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                List<Models.CartSession> cart = System.Text.Json.JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

                var check = cart.SingleOrDefault(o => o.ProductId == id);

                //nếu cart đã có sẵn sản phẩm => thêm quantity
                if (check != null)
                {
                    check.Quantity += 1;
                    check.Price += addedProduct.UnitPrice;
                }
                //cart chưa có => add mới 
                else
                {
                    cart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                }
                HttpContext.Session.SetString("CartList", System.Text.Json.JsonSerializer.Serialize(cart));

            }
            else
            {
                List<Models.CartSession> newCart = new List<Models.CartSession>();
                newCart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                HttpContext.Session.SetString("CartList", System.Text.Json.JsonSerializer.Serialize(newCart));
            }
            return Redirect("/CartProduct/Index");
        }
        [HttpGet]
        public async Task<List<ClientSever.Models.Product>> GetAllProducts()
        {
            products = new List<ClientSever.Models.Product>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://localhost:5000/api/Products/getAllProduct"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ClientSever.Models.Product>>(apiResponse);
                }
            }
            return products.ToList();
        }
    }
}