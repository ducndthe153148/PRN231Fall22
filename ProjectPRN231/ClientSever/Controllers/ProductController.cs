using ClientSever.DTO;
using ClientSever.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClientSever.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ManageProduct()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                ViewBag.Message = TempData["MessSuccess"];
                ViewBag.Mess = TempData["MessExcel"];
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    IEnumerable<Product> products = null;
                    IEnumerable<Category> categories = null;
                    using (var client = new HttpClient())
                    {
                        var accessToken = HttpContext.Session.GetString("token");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);

                        var result = await client.GetAsync("Products/getAllProduct");
                        var getCategory = await client.GetAsync("Categories/ListCategory");
                        if (result.IsSuccessStatusCode)
                        {
                            products = await result.Content.ReadAsAsync<IList<Product>>();
                            if (HttpContext.Session.GetString("listOrder") != null)
                            {
                                HttpContext.Session.Remove("listOrder");
                            }
                            HttpContext.Session.SetString("listOrder", System.Text.Json.JsonSerializer.Serialize(products));
                            
                            categories = await getCategory.Content.ReadAsAsync<IList<Category>>(); 
                            ViewData["products"] = products;

                            ViewBag.ListCategory = new SelectList(categories, "CategoryId", "CategoryName") ;
                        }
                        else
                        {
                            products = Enumerable.Empty<Product>();
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
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
        [HttpPost]
        public async Task<ActionResult> ManageProduct(int CategoryId)
        {
            ViewBag.Message = "Category: " + CategoryId;

            IEnumerable<Product> products = null;
            IEnumerable<Category> categories = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);

                var result = await client.GetAsync("Products/filterCategory/"+CategoryId);
                var getCategory = await client.GetAsync("Categories/ListCategory");
                if (result.IsSuccessStatusCode)
                {
                    products = await result.Content.ReadAsAsync<IList<Product>>();
                    if (HttpContext.Session.GetString("listOrder") != null)
                    {
                        HttpContext.Session.Remove("listOrder");
                    }
                    HttpContext.Session.SetString("listOrder", System.Text.Json.JsonSerializer.Serialize(products));
                    categories = await getCategory.Content.ReadAsAsync<IList<Category>>();
                    ViewData["products"] = products;
                    ViewBag.ListCategory = new SelectList(categories, "CategoryId", "CategoryName",CategoryId);
                }
                else
                {
                    products = Enumerable.Empty<Product>();
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> EditView (int id)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    Product product = null;
                    IEnumerable<Category> categories = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        var result = await client.GetAsync("Products/GetByProdId/" + id);
                        var getCategory = await client.GetAsync("Categories/ListCategory");
                        if (result.IsSuccessStatusCode)
                        {
                            product = await result.Content.ReadAsAsync<Product>();
                            categories = await getCategory.Content.ReadAsAsync<IList<Category>>();
                            ViewBag.ProductEdit = product;
                            ViewBag.ListCategory = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
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
        [HttpPost]
        public async Task<IActionResult> EditDetailProduct()
        {
            var id = Request.Form["ProductId"];
            try
            {
                var name = Request.Form["ProductName"];
                var categoryId = Request.Form["CategoryId"];
                var quantity = Request.Form["QuantityPerUnit"];
                var unitPrice = Request.Form["UnitPrice"];

                await EditProduct(new ProductEdit
                {
                    ProductId = Convert.ToInt32(id),
                    ProductName = name,
                    CategoryId = Convert.ToInt32(categoryId),
                    QuantityPerUnit = quantity,
                    UnitPrice = Convert.ToDecimal(unitPrice),
                });

                // Mess edit success 
                TempData["MessSuccess"] = "1";
            }
            catch (Exception ex)
            {
                // Mess edit fail 
                TempData["MessSuccess"] = "2";
            }
            return RedirectToAction("DetailProduct", "Product", new { id = id });
        }
        public async Task<ActionResult> DetailProduct (int id)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                ViewBag.Message = TempData["MessSuccess"];
                var role = HttpContext.Session.GetString("role");
                if (HttpContext.Session.GetString("role") == "1")
                {
                    Product product = null;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);
                        var result = await client.GetAsync("Products/GetByProdId/"+id);
                        if (result.IsSuccessStatusCode)
                        {
                            product = await result.Content.ReadAsAsync<Product>();
                            ViewData["product"] = product;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
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

        [HttpPut]
        public async Task EditProduct(ProductEdit product)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PutAsJsonAsync<ProductEdit>(_configuration["apiBaseAddress"]+"Products/edit/" + product.ProductId, product))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            // action to delete product
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration["apiBaseAddress"]);

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Products/delete/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["MessSuccess"] = "1";
                } else
                {
                    TempData["MessSuccess"] = "2";
                }
            }
            return RedirectToAction("ManageProduct", "Product");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            List<Product> products = new List<Product>();
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            if (HttpContext.Session.GetString("listOrder") != null)
            {
                products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(HttpContext.Session.GetString("listOrder"), option);
            }
            var account = System.Text.Json.JsonSerializer.Deserialize<ClientSever.Models.Account>(HttpContext.Session.GetString("userAccount"), option);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");
                worksheet.Cell(1, 1).Value = "E-Shop";
                worksheet.Cell(1, 7).Value = "Date: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                worksheet.Cell(3, 3).Value = "Products List";
                var currentRow = 5;
                worksheet.Cell(currentRow, 1).Value = "ProductID";
                worksheet.Cell(currentRow, 2).Value = "ProductName";
                worksheet.Cell(currentRow, 3).Value = "UnitPrice";
                worksheet.Cell(currentRow, 4).Value = "Unit";
                worksheet.Cell(currentRow, 5).Value = "UnitsInStock";
                worksheet.Cell(currentRow, 6).Value = "Category";
                worksheet.Cell(currentRow, 7).Value = "Discontinued";
                foreach (var item in products)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.ProductId;
                    worksheet.Cell(currentRow, 2).Value = item.ProductName;
                    worksheet.Cell(currentRow, 3).Value = item.UnitPrice;
                    worksheet.Cell(currentRow, 4).Value = item.QuantityPerUnit;
                    worksheet.Cell(currentRow, 5).Value = item.UnitsInStock;
                    worksheet.Cell(currentRow, 6).Value = item.Category.CategoryName;
                    worksheet.Cell(currentRow, 7).Value = item.Discontinued;
                }

                worksheet.Cell(currentRow + 3, 7).Value = "Creator";
                worksheet.Cell(currentRow + 4, 7).Value = "Buchanan Steven";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
                }
            }
            //return View("ManageOrder");
        }

        public async Task<IActionResult> DownloadTemplate()
        {
            List<Product> products = new List<Product>();
            products.Add(new Product
            {
                ProductId = 1,
                ProductName = "TestExcel1",
                UnitPrice = 18,
                QuantityPerUnit = "500 ml",
                UnitsInStock = 57,
                CategoryId = 1,
                Discontinued = true,
            });
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "ProductID";
                worksheet.Cell(currentRow, 2).Value = "ProductName";
                worksheet.Cell(currentRow, 3).Value = "UnitPrice";
                worksheet.Cell(currentRow, 4).Value = "Unit";
                worksheet.Cell(currentRow, 5).Value = "UnitsInStock";
                worksheet.Cell(currentRow, 6).Value = "Category";
                worksheet.Cell(currentRow, 7).Value = "Discontinued";
                foreach (var item in products)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.ProductId;
                    worksheet.Cell(currentRow, 2).Value = item.ProductName;
                    worksheet.Cell(currentRow, 3).Value = item.UnitPrice;
                    worksheet.Cell(currentRow, 4).Value = item.QuantityPerUnit;
                    worksheet.Cell(currentRow, 5).Value = item.UnitsInStock;
                    worksheet.Cell(currentRow, 6).Value = item.CategoryId;
                    worksheet.Cell(currentRow, 7).Value = item.Discontinued;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["MessExcel"] = "1";
                return RedirectToAction("ManageProduct", "Product");
            }

            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                TempData["MessExcel"] = "2";
                return RedirectToAction("ManageProduct", "Product");
            }

            var listProduct = new List<Product>();
            using(var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using(var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var product = new Product
                        {
                            ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString().Trim()),
                            QuantityPerUnit = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            UnitsInStock = Convert.ToInt16(worksheet.Cells[row, 5].Value.ToString().Trim()),
                            CategoryId = Convert.ToInt16(worksheet.Cells[row, 6].Value.ToString().Trim()),
                            Discontinued = Convert.ToBoolean(worksheet.Cells[row, 7].Value.ToString().Trim()),
                        };
                        listProduct.Add(product);

                        // call api to add each product
                        await AddProduct(product);
                    }
                }
            }
            
            // Add successfully to list
            TempData["MessExcel"] = "3";
            return RedirectToAction("ManageProduct", "Product");
        }
        [HttpPost]
        public async Task AddProduct(Product product)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsJsonAsync<Product>(_configuration["apiBaseAddress"]+"Products/create", product))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
