using KitchenStock.Api;
using KitchenStock.Api.Helpers;
using KitchenStock.Api.Middlewares;
using KitchenStock.Api.Transformers;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Infrastructure.Configuration;
using KitchenStock.Infrastructure.Modules.Auth;
using KitchenStock.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.Json;
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
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    }).ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = (actionContext) =>
        {
            var response = new InvalidModelStateResponse(actionContext.ModelState);
            return new BadRequestObjectResult(new
            {
                response.Message,
                Errors = response.Errors.Select(e => new
                {
                    e.Message,
                    e.Metadata
                }).ToList()
            });
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IAuthService).Assembly)
);

builder.Services.Configure<KitchenStockSettings>(builder.Configuration.GetSection(KitchenStockSettings.KITCHENSTOCK_SECTION));
builder.Services.AddKitchenStockDbContext();
builder.Services.AddKitchenStockServices(builder.Configuration);
builder.Services.AddJwtAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KitchenStockDbContext>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();
