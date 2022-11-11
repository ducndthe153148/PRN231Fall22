using ClientSever.DTO;
using ClientSever.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SelectPdf;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace ClientSever.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        public OrderController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> ManageOrder()
        {
            //HttpContext.Session.GetString("user") != null
            if (HttpContext.Session.GetString("user") != null)
            {
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    IEnumerable<Order> orders = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        var accessToken = HttpContext.Session.GetString("token");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var result = await client.GetAsync("order/getorders");

                        if (result.IsSuccessStatusCode)
                        {
                            orders = await result.Content.ReadAsAsync<IList<Order>>();
                            if (HttpContext.Session.GetString("listOrder") != null)
                            {
                                HttpContext.Session.Remove("listOrder");
                            }
                            HttpContext.Session.SetString("listOrder", System.Text.Json.JsonSerializer.Serialize(orders));
                        }
                        else
                        {
                            orders = Enumerable.Empty<Order>();
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
                        }
                    }
                    return View(orders);
                }
                else
                {
                    return Redirect("/Welcome/Error");
                }
            }
            else
            {
                return Redirect("/Welcome/SignIn");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ManageOrder(string orderId)
        {
            //HttpContext.Session.GetString("user") != null
            if (HttpContext.Session.GetString("user") != null)
            {
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    var startDate = (Request.Form["txtStartOrderDate"]);
                    var endDate = (Request.Form["txtEndOrderDate"]);

                    IEnumerable<Order> orders = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        var accessToken = HttpContext.Session.GetString("token");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var result = await client.GetAsync("order/filterdate/" + startDate + "/" + endDate);

                        if (result.IsSuccessStatusCode)
                        {
                            orders = await result.Content.ReadAsAsync<IList<Order>>();
                            if (HttpContext.Session.GetString("listOrder") != null)
                            {
                                HttpContext.Session.Remove("listOrder");
                            }
                            HttpContext.Session.SetString("listOrder", System.Text.Json.JsonSerializer.Serialize(orders));
                        }
                        else
                        {
                            orders = Enumerable.Empty<Order>();
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
                        }
                    }
                    return View(orders);
                }
                else
                {
                    return Redirect("/Welcome/Error");
                }
            }
            else
            {
                return Redirect("/Welcome/SignIn");
            }
        }

        public async Task<ActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    ClientSever.Models.Order order = new ClientSever.Models.Order();
                    order = await GetOrdersByOrderId(id);
                    ViewBag.EachOrder = order;

                    return View();
                }
                else
                {
                    return Redirect("/Welcome/Error");
                }
            }
            else
            {
                return Redirect("/Welcome/SignIn");
            }
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceDetail(int id)
        {

            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            if(HttpContext.Session.GetString("userAccount") != null)
            {
                var account = System.Text.Json.JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);
                ViewBag.Account = account;
            }
            var contactName = "";
            if(HttpContext.Session.GetString("ContactNameGuess") != null)
            {
                contactName = HttpContext.Session.GetString("ContactNameGuess");
                HttpContext.Session.Remove("ContactNameGuess");
            }
            ViewBag.ContactName = contactName;
            ClientSever.Models.Order order = new ClientSever.Models.Order();
            order = await GetOrdersByOrderId(id);
            ViewBag.EachOrder = order;
            
            return View();

        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            using (var client = new HttpClient())
            {
                var url = _configuration["apiBaseAddress"] + "Order/CancelOrder/" + id;
                client.BaseAddress = new Uri(url);
                var accessToken = HttpContext.Session.GetString("token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //HTTP Put
                var putTask = client.PutAsJsonAsync(url, id);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ManageOrder");
                }
            }
            return Redirect("/Welcome/Error");
        }

        public async Task<ActionResult> MyOrder()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                try
                {
                    AccCusDTO useAccount = null;
                    IEnumerable<Order> orders = null;
                    using (var client = new HttpClient())
                    {
                        var accountUse = HttpContext.Session.GetString("user");
                        var accessToken = HttpContext.Session.GetString("token");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        var url = "account/GetAccountByEmail/" + accountUse;
                        var result = await client.GetAsync(url);

                        if (result.IsSuccessStatusCode)
                        {
                            useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                            var url2 = "order/GetOrdersByCustomerId/" + useAccount.CustomerId;
                            var result2 = await client.GetAsync(url2);
                            if (result2.IsSuccessStatusCode)
                            {
                                orders = await result2.Content.ReadAsAsync<IList<Order>>();
                                ViewData["orders"] = orders;
                                return View();
                            }
                            else
                            {
                                return Redirect("/Welcome/Error");
                            }
                        }
                        else
                        {
                            return Redirect("/Welcome/Error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Redirect("/Welcome/Error");
                }

            }
            else
            {
                return Redirect("/Welcome/SignIn");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterDate(DateTime from, DateTime to)
        {

            return Redirect("/Order/ManageOrder");
        }

        [HttpGet]
        public async Task<ClientSever.Models.Order> GetOrdersByOrderId(int id)
        {
            ClientSever.Models.Order products = new ClientSever.Models.Order();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Order/GetOrderByOrderId/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<ClientSever.Models.Order>(apiResponse);
                }
            }
            return products;
        }
        public async Task<IActionResult> ExportToExcel()
        {
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var curUser = HttpContext.Session.GetString("userAccount");
            if(curUser != null)
            {
                var curAccount = System.Text.Json.JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);
                EmailSender emailSender = new EmailSender();
                var email = curAccount.Email;
                var account = new Account();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Account/CheckEmail/" + email))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        account = JsonConvert.DeserializeObject<Account>(apiResponse);
                    }
                }
                await emailSender.SendMailPdf(account);
                return RedirectToAction("Profile","Welcome");
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
