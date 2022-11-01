using ClientSever.DTO;
using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace ClientSever.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        readonly string apiBaseAddress = "http://localhost:5000/api/";
        public async Task<ActionResult> ManageOrder()
        {
            //HttpContext.Session.GetString("user") != null
            if(HttpContext.Session.GetString("user") != null)
            {
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    IEnumerable<Order> orders = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiBaseAddress);

                        var result = await client.GetAsync("order/getorders");

                        if (result.IsSuccessStatusCode)
                        {
                            orders = await result.Content.ReadAsAsync<IList<Order>>();
                        }
                        else
                        {
                            orders = Enumerable.Empty<Order>();
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
                        }
                    }
                    return View(orders);
                } else
                {
                    return Redirect("/Welcome/Error");
                }
            } else
            {
                return Redirect("/Welcome/SignIn");
            }
            
        }
        
        public async Task<ActionResult> Detail(int id)
        {
            return View();
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
                        client.BaseAddress = new Uri(apiBaseAddress);
                        var url = "account/GetAccountByEmail/" + accountUse;
                        var result = await client.GetAsync(url);

                        if (result.IsSuccessStatusCode)
                        {
                            useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                            var url2 = "order/GetOrdersByCustomerId/" + useAccount.CustomerId;
                            var result2 = await client.GetAsync(url2);
                            if(result2.IsSuccessStatusCode)
                            {
                                orders = await result2.Content.ReadAsAsync<IList<Order>>();
                                ViewData["orders"] = orders;
                                return View();
                            } else
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
    }
}
