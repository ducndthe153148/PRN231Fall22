using ClientSever.DTO;
using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ClientSever.Controllers
{
    public class WelcomeController : Controller
    {
        readonly string apiBaseAddress = "http://localhost:5000/api/";
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("userAccount");

            return View();
        }
        public IActionResult SignUp()
        {
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("userId");

            return View();
        }
        public async Task<IActionResult> Profile()
        {
            if(HttpContext.Session.GetString("user") != null)
            {
                try
                {
                    AccCusDTO useAccount = null;
                    using (var client = new HttpClient())
                    {
                        var accountUse = HttpContext.Session.GetString("user");
                        client.BaseAddress = new Uri(apiBaseAddress);
                        var url = "account/GetAccountByEmail/" + accountUse;
                        var result = await client.GetAsync(url);

                        if (result.IsSuccessStatusCode)
                        {
                            useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                            ViewData["useAccount"] = useAccount;
                            return View();
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
                
            } else
            {
                return Redirect("/Welcome/SignIn");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckSignIn(Account account)
        {
            if(account != null)
            {
                string data = JsonConvert.SerializeObject(account);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                AccCusDTO useAccount = null;
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    HttpResponseMessage response = client.PostAsync(client.BaseAddress + "Account/login", content).Result;
                    var url = "account/GetAccountByEmail/" + account.Email;
                    var result = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                        HttpContext.Session.SetString("user", account.Email);
                        HttpContext.Session.SetString("role", useAccount.Role.ToString());
                        HttpContext.Session.SetString("userId", useAccount.AccountId.ToString());
                        HttpContext.Session.SetString("userAccount", JsonSerializer.Serialize(useAccount));
                        if (useAccount.Role.ToString() == "1")
                        {
                            return Redirect("/Order/ManageOrder");
                        }
                        return Redirect("/Home/Index");
                    } else
                    {
                        return Redirect("/Welcome/SignIn");
                    }
                }
            } else
            {
                return Redirect("/Welcome/SignIn");
            }
        }
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CartList");
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role"); 
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("userAccount");
            return RedirectToAction("SignIn", "Welcome");
        }
    }
}
