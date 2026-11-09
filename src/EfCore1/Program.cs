using EfCore1.Data;
using EfCore1.Files;
using EfCore1.Middlewares;
using EfCore1.Repositories;
using EfCore1.Seeding;
using EfCore1.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(
    option =>
    {
        option.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path.Value;

            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }
);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Repository
// builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

// EFCORE REPOSITORY
builder.Services.AddScoped<IProductRepository, EfProductRepository>();

// Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageStorage, LocalProductImageStorage>();

var connectionString = builder.Configuration.GetConnectionString("ecommerce")
        ?? throw new InvalidOperationException("ConnectionString : Belum dikonfigurasi");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.UseSqlServer(connectionString);
    options.UseSqlServer(connectionString)
       .UseSeeding((context, _) => ProductSeedData.Seed(context))
       .UseAsyncSeeding((context, _, ct) => ProductSeedData.SeedAsync(context, ct));
});


var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
     {
         options.SwaggerEndpoint("/openapi/v1.json", "Koda Store API v1");
         options.RoutePrefix = "swagger";

         options.DocumentTitle = "Koda Store | API Documentation";

         options.EnableFilter();
     });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
