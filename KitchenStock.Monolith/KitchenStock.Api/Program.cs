using KitchenStock.Api;
using KitchenStock.Api.Helpers;
using KitchenStock.Api.Middlewares;
using KitchenStock.Api.Transformers;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using KitchenStock.Infrastructure.Modules.Auth.Extensions;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
    options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    }).ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = (actionContext) =>
        {
            var response = new InvalidModelStateResponse(actionContext.ModelState);
            return new BadRequestObjectResult(new
            {
                message = response.Message,
                errors = response.Errors.Select(e => new
                {
                    message = e.Message,
                    metadata = e.Metadata
                }).ToList()
            });
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IAuthService).Assembly)
);

builder.Services.Configure<KitchenStockSettings>(builder.Configuration.GetSection(KitchenStockSettings.KITCHENSTOCK_SECTION));
builder.Services.AddKitchenStockLogging();
builder.Services.AddKitchenStockDbContext();
builder.Services.AddKitchenStockServices(builder.Configuration);
builder.Services.AddKitchenStockBackgroundServices();
await builder.Services.AddJwtAuthenticationAsync();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowCredentials()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<MediaTypeResponseMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KitchenStockDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
    
    await context.Database.EnsureCreatedAsync();
    await DatabaseSeeder.SeedAsync(context, logger);
}

await app.RunAsync();
