using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using Use_API.Models;

namespace Use_API.Pages.Employees
{
    public class EditModel : PageModel
    {


        private PRN231DBContext _context = new PRN231DBContext();
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";

        [BindProperty]
        public Employee Employee { get; set; }
        public EditModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/employee/";
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{EmployeeApiUrl}GetById/{id}");
            string stringData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Employee = JsonSerializer.Deserialize<Employee>(stringData, options);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            bool check = false;

            //if (ModelState.IsValid)
            //{
            string stringData = JsonSerializer.Serialize(Employee);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(EmployeeApiUrl, contentData);
            if (response.IsSuccessStatusCode)
            {
                check = true;
            }
            Console.WriteLine("Error: ");
            //}
            if (check)
                return RedirectToPage("/Employees/Index");
            return RedirectToPage("/Error");
        }
    }
}
