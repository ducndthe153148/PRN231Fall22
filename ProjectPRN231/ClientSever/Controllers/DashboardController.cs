using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace ClientSever.Controllers
{
    public class DashboardController : Controller
    {
        private IConfiguration _configuration;
        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> ShowData(int year = 2022)
        {
            // Check login and not show for customer 
            if (HttpContext.Session.GetString("user") != null)
            {
                IEnumerable<Order> listOrder = null;
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    List<int> listYear;
                    using (var client = new HttpClient())
                    {
                        var accessToken = HttpContext.Session.GetString("token");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);

                        var totalCustomer = await client.GetAsync("Customers/GetTotalCustomer");
                        var totalGuest = await client.GetAsync("Customers/GetTotalGuest");
                        var totalOrders = await client.GetAsync("Customers/GetTotalOrders");
                        var weeklySale = await client.GetAsync("Customers/GetWeeklySale");
                        var resultOrder = await client.GetAsync("Order/GetAllOrders");
                        var resultYear = await client.GetAsync("Order/GetYear");
                        var dictionary = new Dictionary<int, string>();
                        
                        listYear = await resultYear.Content.ReadAsAsync<List<int>>();
                        
                        foreach (var item in listYear)
                        {
                            dictionary.Add(item, item.ToString());
                        }

                        ViewBag.ListYear = new SelectList(dictionary, "Key", "Value", year);

                        listOrder = await resultOrder.Content.ReadAsAsync<IList<Order>>();
                        // order month
                        int Jan = 0;
                        int Feb = 0;
                        int Mar = 0;
                        int Apr = 0;
                        int May = 0;
                        int Jun = 0;
                        int Jul = 0;
                        int Aug = 0;
                        int Sep = 0;
                        int Oct = 0;
                        int Nov = 0;
                        int Dec = 0;

                        List<ClientSever.Models.Order> ordersJan = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 1
                            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersJan)
                        {
                            Jan += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersFeb = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 2
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersFeb)
                        {
                            Feb += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersMarch = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 3
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersMarch)
                        {
                            Mar += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersApr = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 4
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersApr)
                        {
                            Apr += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersMay = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 5
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersMay)
                        {
                            May += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersJun = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 6
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersJun)
                        {
                            Jun += Convert.ToInt32(item.Freight);
                        }
                        List<ClientSever.Models.Order> ordersJul = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 7
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersJul)
                        {
                            Jul += Convert.ToInt32(item.Freight);
                        }
                        List<ClientSever.Models.Order> ordersAug = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 8
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersAug)
                        {
                            Aug += Convert.ToInt32(item.Freight);
                        }
                        List<ClientSever.Models.Order> ordersSep = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 9
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersSep)
                        {
                            Sep += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersOct = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 10
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersOct)
                        {
                            Oct += Convert.ToInt32(item.Freight);
                        }

                        List<ClientSever.Models.Order> ordersNov = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 11
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersNov)
                        {
                            Nov += Convert.ToInt32(item.Freight);
                        }
                        List<ClientSever.Models.Order> ordersDec = listOrder.Where(o => o.OrderDate.GetValueOrDefault().Month == 12
                        && o.OrderDate.GetValueOrDefault().Year == year).ToList();
                        foreach (var item in ordersDec)
                        {
                            Dec += Convert.ToInt32(item.Freight);
                        }
                        ViewBag.Jan = Jan;
                        ViewBag.Feb = Feb;
                        ViewBag.Mar = Mar;
                        ViewBag.Apr = Apr;
                        ViewBag.May = May;
                        ViewBag.Jun = Jun;
                        ViewBag.Jul = Jul;
                        ViewBag.Aug = Aug;
                        ViewBag.Sep = Sep;
                        ViewBag.Oct = Oct;
                        ViewBag.Nov = Nov;
                        ViewBag.Dec = Dec;

                        if (totalCustomer.IsSuccessStatusCode)
                        {
                            ViewData["WeeklySale"] = await weeklySale.Content.ReadAsStringAsync();
                            ViewData["TotalCustomer"] = await totalCustomer.Content.ReadAsStringAsync();
                            ViewData["TotalOrder"] = await totalOrders.Content.ReadAsStringAsync();
                            ViewData["TotalGuest"] = await totalGuest.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            return Redirect("/Welcome/Error");
                        }
                    }
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
        public async Task<ClientSever.Models.NewCustomer> GetCustomerData()
        {
            ClientSever.Models.NewCustomer customer = new ClientSever.Models.NewCustomer();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);

                var result = await client.GetAsync("Customers/GetNewCustomer");

                if (result.IsSuccessStatusCode)
                {
                    customer = await result.Content.ReadAsAsync<ClientSever.Models.NewCustomer>();
                }
                else
                {
                    customer.New = 30;
                    customer.Total = 200;
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            return customer;
        }
    }
}
