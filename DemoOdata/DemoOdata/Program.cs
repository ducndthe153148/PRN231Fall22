

using DemoOdata.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);
static IEdmModel GetEdmModel() {
    ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Gadget>("GadgetsOdata");
    return modelBuilder.GetEdmModel();
}
builder.Services.AddControllers
    (options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddMvc();
builder.Services.AddCors();
builder.Services.AddDbContext<OdataASPNetContext>();

builder.Services.AddControllers()
    .AddOData
    (
    option => option.Select().Filter().Count().OrderBy()
    .Expand().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel())
    );

var app = builder.Build();
app.UseCors();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
