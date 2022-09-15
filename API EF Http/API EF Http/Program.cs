using API_EF_Http.DataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddCors();
builder.Services.AddDbContext<PRN231DBContext>();
builder.Services.AddControllers
    (options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

var app = builder.Build();
app.UseCors();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
