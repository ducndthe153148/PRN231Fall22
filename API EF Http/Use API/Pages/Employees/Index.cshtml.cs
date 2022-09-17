using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using Use_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Use_API.Pages.Employees
{
    public class IndexModel : PageModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        private PRN231DBContext _context = new PRN231DBContext();
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";

        [BindProperty]
        public Employee Employee { get; set; }
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/employee/";
        }
        public async Task<IActionResult> OnGet()
        {
            HttpResponseMessage response = await client.GetAsync(EmployeeApiUrl);
            string stringData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
            Employees = JsonSerializer.Deserialize<List<Employee>>(stringData, options);
            
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            // Delete from DB
            HttpResponseMessage response = await client.DeleteAsync($"{EmployeeApiUrl}{id}");

            // page with employee data

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees/Index");
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
