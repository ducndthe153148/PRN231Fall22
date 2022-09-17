using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Net.Http.Headers;
using System.Text.Json;
using Use_API.Models;

namespace Use_API.Pages.Employees
{
    public class CreateModel : PageModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        private PRN231DBContext _context = new PRN231DBContext();
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";
        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/employee/";
        }
        public void OnGet()
        {
        }
        [BindProperty]
        public Employee Employee { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            bool check = false;
            
            //if (ModelState.IsValid)
            //{
                string stringData = JsonSerializer.Serialize(Employee);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(EmployeeApiUrl, contentData);
                if (response.IsSuccessStatusCode)
                {
                    check = true;
                }
                Console.WriteLine("Error: ");
            //}
            if(check)
                return RedirectToPage("/Employees/Index");
            return RedirectToPage("/Error");
        }
        
    }
}
