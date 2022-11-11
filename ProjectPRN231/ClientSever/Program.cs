using ClientSever.DTO;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var apiBaseAddress = builder.Configuration["apiBaseAddress"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Welcome/Error");
}
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 400 || context.Response.StatusCode == 401 || context.Response.StatusCode == 402 || context.Response.StatusCode == 403 || context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Welcome/Error";
        await next();
    }
});

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
