using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ClientSever.Controllers
{
    public class CartProductController : Controller
    {
        public List<Models.CartSession> cart;
        public decimal? TotalMoney;
        public ClientSever.Models.Customer existedCustomer;
        private IConfiguration _configuration;
        public CartProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            cart = new List<Models.CartSession>();
            List<ClientSever.Models.Customer> customerList = await GetAllCustomer();
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            existedCustomer = new Models.Customer();
            TotalMoney = 0;
            // fill existed customer field
            if (HttpContext.Session.GetString("userAccount") != null)
            {
                var account = JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);
                existedCustomer = customerList.SingleOrDefault(o => o.CustomerId == account.CustomerId);
            }
            if (HttpContext.Session.GetString("CartList") != null)
            {

                cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
                ViewBag.Cart = cart;
            }
            else
            {
                ViewBag.Cart = null;

            }
            ViewBag.TotalMoney = TotalMoney;
            ViewBag.ExistedCustomer = existedCustomer;

            return View();
        }
        public async Task<IActionResult> PlusProduct(int id)
        {
            cart = new List<Models.CartSession>();
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var plusPro = cart.SingleOrDefault(o => o.ProductId == id);
            List<ClientSever.Models.Product> products = await GetAllProducts();
            var productInStock = products.SingleOrDefault(o => o.ProductId == id);
            if (plusPro.Quantity == productInStock.UnitsInStock)
            {
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
                return RedirectToAction("Index");
            }
            else
            {
                plusPro.Quantity += 1;
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }

            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MinusProduct(int id)
        {
            cart = new List<Models.CartSession>();
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var minusPro = cart.SingleOrDefault(o => o.ProductId == id);
            if (minusPro.Quantity == 1)
            {
                cart.Remove(minusPro);
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }
            else
            {
                minusPro.Quantity -= 1;
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }

            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveProduct(int id)
        {
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var removePro = cart.SingleOrDefault(o => o.ProductId == id);
            cart.Remove(removePro);
            foreach (var item in cart)
            {
                TotalMoney += (item.Price * item.Quantity);
            }
            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Order()
        {
            var requiredDate = Convert.ToDateTime(Request.Form["requiredDate"]);

            // check guess or customer
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            decimal money = Convert.ToDecimal(Request.Form["totalCart"]);
            // Customer
            if (HttpContext.Session.GetString("userAccount") != null)
            {
                var account = JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);

                //add order
                int orderId1 = await CreateOrder(new OrderAdd
                {
                    CustomerId = account.CustomerId,
                    Freight = money,
                    RequiredDate = requiredDate,
                    ShippedDate = requiredDate
                });

                // add order detail
                cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
                foreach (var item in cart)
                {
                    await AddOrderDetail(new OrderDetailAdd
                    {
                        Discount = 0,
                        ProductId = item.ProductId,
                        OrderId = orderId1,
                        Quantity = Convert.ToInt16(item.Quantity),
                        UnitPrice = (decimal)item.Price
                    });
                }
                HttpContext.Session.Remove("CartList");
                // Redirec to invoice detail
                return RedirectToAction("InvoiceDetail", "Order", new {id = orderId1});
            }

            //Guess

            // Add guess

            var CompanyName = Request.Form["CompanyName"];
            var ContactName = Request.Form["ContactName"];
            var ContactTitle = Request.Form["ContactTitle"];
            var Address = Request.Form["Address"];
            HttpContext.Session.SetString("ContactNameGuess", ContactName);
            string cusId = await CreateGuess(new GuessAdd
            {
                Address = Address,
                ContactName = ContactName,
                ContactTitle = ContactTitle,
                CompanyName = CompanyName,
            });

            //add order
            int orderId = await CreateOrder(new OrderAdd
            {
                CustomerId = cusId,
                Freight = money,
                RequiredDate = requiredDate,
                ShippedDate = requiredDate,
            });

            //Add order
            cart = JsonSerializer.Deserialize<List<ClientSever.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
            foreach (var item in cart)
            {
                await AddOrderDetail(new OrderDetailAdd
                {
                    Discount = 0,
                    ProductId = item.ProductId,
                    OrderId = orderId,
                    Quantity = Convert.ToInt16(item.Quantity),
                    UnitPrice = (decimal)item.Price
                });
            }
            HttpContext.Session.Remove("CartList");

            return RedirectToAction("InvoiceDetail", "Order", new { id = orderId });
        }

        [HttpGet]
        public async Task<List<ClientSever.Models.Customer>> GetAllCustomer()
        {
            List<ClientSever.Models.Customer> customerList = new List<ClientSever.Models.Customer>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Customers/GetCustomers"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    customerList = JsonConvert.DeserializeObject<List<ClientSever.Models.Customer>>(apiResponse);

                }
            }
            return customerList.ToList();
        }
        [HttpGet]
        public async Task<List<ClientSever.Models.Product>> GetAllProducts()
        {
            List<ClientSever.Models.Product> products = new List<ClientSever.Models.Product>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Products/getAllProduct"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ClientSever.Models.Product>>(apiResponse);
                }
            }
            return products.ToList();
        }

        [HttpPost]
        public async Task<int> CreateOrder(OrderAdd order)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsJsonAsync<OrderAdd>(_configuration["apiBaseAddress"]+"Order", order))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return Convert.ToInt32(apiResponse);
        }

        [HttpPost]
        public async Task AddOrderDetail(OrderDetailAdd order)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsJsonAsync<OrderDetailAdd>(_configuration["apiBaseAddress"]+"OrderDetails", order))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }

        [HttpPost]
        public async Task<string> CreateGuess(GuessAdd order)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsJsonAsync<GuessAdd>(_configuration["apiBaseAddress"]+"Customers", order))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return apiResponse;
        }
    }
}
