using ClientSever.DTO;
using ClientSever.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ClientSever.Controllers
{
    public class WelcomeController : Controller
    {
        private IConfiguration _configuration;
        public WelcomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
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
            HttpContext.Session.Remove("token");
            ViewBag.Mess = TempData["MessSuccess"];
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                try
                {
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

            }
            else
            {
                return Redirect("/Welcome/SignIn");
            }
        }
        
        public async Task<IActionResult> CheckSignIn(Account account)
        {

            //    if (account != null)
            //{
            if(account.Email == null || account.Password == null)
            {
                return View("SignIn");
            }
                string data = JsonConvert.SerializeObject(account);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    AccCusDTO useAccount = null;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        HttpResponseMessage response = client.PostAsync(client.BaseAddress + "Account/login", content).Result;
                        var url = "account/GetAccountByEmail/" + account.Email;
                        var result = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            string jwtToken = response.Content.ReadAsStringAsync().Result;
                            HttpContext.Session.SetString("token", jwtToken);
                            useAccount = await result.Content.ReadAsAsync<AccCusDTO>();
                            HttpContext.Session.SetString("user", account.Email);
                            HttpContext.Session.SetString("role", useAccount.Role.ToString());
                            HttpContext.Session.SetString("userId", useAccount.AccountId.ToString());
                            HttpContext.Session.SetString("userAccount", JsonSerializer.Serialize(useAccount));
                            //EmailSender emailSender = new EmailSender();
                            //await emailSender.ForgetPassword();
                            if (useAccount.Role.ToString() == "1")
                            {
                                return Redirect("/Order/ManageOrder");
                            }
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            TempData["MessSuccess"] = "1";
                            return Redirect("/Welcome/SignIn");
                        }
                    }
                //}
            
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
            HttpContext.Session.Remove("token");

            return RedirectToAction("SignIn", "Welcome");
        }
        public IActionResult SignUp()
        {
            HttpContext.Session.Remove("CartList");
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("userAccount");
            HttpContext.Session.Remove("token");

            return View();
        }
        public async Task<IActionResult> SignupAccount(AccCusDTO customer)
        {
            var rePassword = Request.Form["rePassword"];
            if (customer.Password != rePassword)
            {
                ViewBag.Mess = "2";
                return View("SignUp");
            }

            List<ClientSever.Models.Account> accounts = await GetAllAccounts();
            foreach (var account in accounts)
            {
                if (customer.Email.Trim().ToLower().Equals(account.Email.Trim().ToLower()))
                {
                    ViewBag.Mess = "1";
                    return View("SignUp");
                }
            }

            await AddAccount(new AccCusDTO
            {
                Address = customer.Address,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                ContactTitle = customer.ContactTitle,
                Email = customer.Email,
                Password = customer.Password,
                Role = 2
            });

            return RedirectToAction("SignIn", "Welcome");
        }

        // Chua fix
        public async Task<IActionResult> EditProfileView()
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
            ViewBag.Account = acc;
            List<ClientSever.Models.Customer> customers = await GetAllCustomers();
            ViewBag.Customer = customers.SingleOrDefault(o => o.CustomerId == acc.CustomerId);
            return View();
        }
        // Chua fix
        [HttpPost]
        public async Task<IActionResult> EditUserProfile()
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
            List<ClientSever.Models.Customer> customers = await GetAllCustomers();
            var editedCustomer = customers.SingleOrDefault(o => o.CustomerId == acc.CustomerId);

            var CompanyName = Request.Form["CompanyName"];
            var ContactName = Request.Form["ContactName"];
            var CompanyTitle = Request.Form["CompanyTitle"];
            var Address = Request.Form["Address"];
            var Email = Request.Form["Email"];

            // Check email neu trung thi redirect lai ve edit 
            //List<ClientSever.Models.Account> accounts = await GetAllAccounts();
            //foreach (var account in accounts)
            //{
            //    if (Email.Equals(account.Email.Trim().ToLower()))
            //    {
            //        ViewBag.Mess = "1";
            //        return View("EditProfileView");
            //    }
            //}

            await EditProfile(new CustomerEdit
            {
                CustomerId = editedCustomer.CustomerId,
                Address = Address,
                CompanyName = CompanyName,
                ContactName = ContactName,
                ContactTitle = CompanyTitle,
                // Email = Email
            });

            return RedirectToAction("Profile", "Welcome");
        }

        [HttpPut]
        public async Task EditProfile(CustomerEdit customer)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PutAsJsonAsync<CustomerEdit>(_configuration["apiBaseAddress"]+"Customers/" + customer.CustomerId, customer))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }

        [HttpPost]
        public async Task AddAccount(AccCusDTO customer)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsJsonAsync<AccCusDTO>(_configuration["apiBaseAddress"] + "Account/Register", customer))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }

        [HttpGet]
        public async Task<List<ClientSever.Models.Account>> GetAllAccounts()
        {
            List<ClientSever.Models.Account> accounts = new List<ClientSever.Models.Account>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Account/GetAllAccounts"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    accounts = JsonConvert.DeserializeObject<List<ClientSever.Models.Account>>(apiResponse);
                }
            }
            return accounts.ToList();
        }

        [HttpGet]
        public async Task<List<ClientSever.Models.Customer>> GetAllCustomers()
        {
            List<ClientSever.Models.Customer> products = new List<ClientSever.Models.Customer>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"]+"Customers/GetCustomers"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ClientSever.Models.Customer>>(apiResponse);
                }
            }
            return products.ToList();
        }
        [HttpGet]
        public async Task<IActionResult> ForgotPasswordView()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword()
        {
            var email = Request.Form["email"];
            var account = new Account();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_configuration["apiBaseAddress"] +"Account/CheckEmail/" + email))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    account = JsonConvert.DeserializeObject<Account>(apiResponse);
                }
            }
            if (email == "")
            {
                // khong dien mail
                ViewBag.Mess = "1";
                return View("ForgotPasswordView");
            }
            if (account.Email == null)
            {
                // dien mail nhung k co mail trong db
                ViewBag.Mess = "2";
                return View("ForgotPasswordView");
            }
            else
            {
                // dien dung mail, tien hanh gui email reset password
                var token = RandomString(15);
                EmailSender emailSender = new EmailSender();
                await emailSender.ForgetPassword(token, account);
                // Thong bao gui mail thanh cong
                ViewBag.Mess = "3";
                HttpContext.Session.SetString("EmailChangePass", email);
                return View("ForgotPasswordView");
            }
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token = "")
        {
            var email = HttpContext.Session.GetString("EmailChangePass");
            // send token to form if necessary
            if (email != null)
            {
                ViewBag.Email = email;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword()
        {
            // Get token from form to redirect to view  
            var email = HttpContext.Session.GetString("EmailChangePass");
            var password = Request.Form["password"];
            var rePassword = Request.Form["rePassword"];

            if (password != rePassword)
            {
                ViewBag.Mess = "1"; 
            } else
            {
                Account account = new Account { Email = email, Password = password};
                // Call api to change password
                string apiResponse = "";
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PutAsJsonAsync<Account>(_configuration["apiBaseAddress"] + "Account/ChangePassword", account))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                // Change success fully
                ViewBag.Mess = "2";
                HttpContext.Session.Remove("EmailChangePass");
            }

            return View("ResetPassword");
        }

        private string RandomString(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
    }
}
