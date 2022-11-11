using ClientSever.DTO;
using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ClientSever.Controllers
{
    public class OrderViewController : Controller
    {
        private IConfiguration _configuration;
        public OrderViewController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            if (HttpContext.Session.GetString("userAccount") == null)
            {
                return RedirectToAction("Error", "Welcome");
            }
            Account acc = JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);

            ClientSever.Models.Customer customer = new ClientSever.Models.Customer();
            customer = await GetAllOrdersByCustomer(acc.CustomerId);

            ViewBag.CustomerOrder = customer;

            AccCusDTO useAccount = null;
            using (var client = new HttpClient())
            {
                var accountUse = HttpContext.Session.GetString("user");
                client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                var url = "account/GetAccountByEmail/" + accountUse;
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                    ViewData["useAccount"] = useAccount;
                }
                else
                {
                    return Redirect("/Welcome/Error");
                }
            }

            return View();
        }
        public async Task<IActionResult> CanceledOrder()
        {

            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            if (HttpContext.Session.GetString("userAccount") == null)
            {
                return RedirectToAction("Error", "Welcome");
            }
            Account acc = JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);

            ClientSever.Models.Customer customer = new ClientSever.Models.Customer();
            customer = await GetAllOrdersByCustomer(acc.CustomerId);

            ViewBag.CustomerOrder = customer;

            AccCusDTO useAccount = null;
            using (var client = new HttpClient())
            {
                var accountUse = HttpContext.Session.GetString("user");
                client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                var url = "account/GetAccountByEmail/" + accountUse;
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                    ViewData["useAccount"] = useAccount;
                }
                else
                {
                    return Redirect("/Welcome/Error");
                }
            }

            return View();
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            using (var client = new HttpClient())
            {
                var url = _configuration["apiBaseAddress"] + "Order/CancelOrder/" + id;
                client.BaseAddress = new Uri(url);

                //HTTP POST
                var putTask = client.PutAsJsonAsync(url, id);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return Redirect("/Welcome/Error");
        }

        [HttpGet]
        public async Task<ClientSever.Models.Customer> GetAllOrdersByCustomer(string id)
        {
            ClientSever.Models.Customer products = new ClientSever.Models.Customer();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Order/GetOrdersByCustomerId/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<ClientSever.Models.Customer>(apiResponse);
                }
            }
            return products;
        }
    }
}
