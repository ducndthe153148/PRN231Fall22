using API_EF_Http.DataAccess;
using API_EF_Http.DTO;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers
    (options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddMvc();
builder.Services.AddCors();
builder.Services.AddDbContext<PRN231DBContext>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//var config = new AutoMapper.MapperConfiguration(cfg =>
//{
//    cfg.AddProfile(new MappingDTO());
//});

//var mapper = config.CreateMapper();
//builder.Services.AddSingleton(mapper);

var app = builder.Build();
app.UseCors();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
