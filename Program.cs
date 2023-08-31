global using Agriculture.Data;
global using Agriculture.Models;
global using Agriculture.Helpers;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using Agriculture.Services.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddDbContext<DataContext>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<CouponsService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ShippingService>();
builder.Services.AddScoped<ReviewsService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<TransferPointService>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<Token>();
builder.Services.AddScoped<CheckFile>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>

{

    opt.TokenValidationParameters = new
    TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8
    .GetBytes("ProjectReactNativeSecretKeyToken"))
    };
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder =>
//{
//    containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
//        .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Test"))
//        .AsSelf()
//        .InstancePerLifetimeScope();

//}));

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();

using var scope = app.Services.CreateScope(); //using หลังท างานเสร็จจะถูกท าลายจากMemory
var context = scope.ServiceProvider.GetRequiredService<DataContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync(); //สร้าง DB ให้อัตโนมัติถ้ายังไม่มี
    
}
catch (Exception ex)
{
    logger.LogError(ex, "Problem migrating data");
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
