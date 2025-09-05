using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Common.Middleware;
using Shared.Common.Vault.Abstractions;
using Shared.Common.Vault.Extensions;
using Shared.Common.Vault.Settings;
using User.Application.Commands;
using User.Application.Services.Abstractions;
using User.Infrastructure;
using User.Infrastructure.Authentication;
using User.Infrastructure.Models;
using User.Infrastructure.Repositories;
using User.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly)
);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<UserDbContext>((sp, options) => 
{
    var vault = sp.GetRequiredService<IVaultService>();
    var vaultConString = vault.GetAsync<VaultDbConnectionString>("kitchen/services/user/db/dev").GetAwaiter().GetResult();

    if (vaultConString is null || string.IsNullOrEmpty(vaultConString.ConnectionString))
        throw new InvalidOperationException("Database connection string is missing in Vault");

    options.UseNpgsql(vaultConString.ConnectionString);
});

var vaultSettings = builder.Configuration.GetSection("Vault").Get<VaultSettings>();
builder.Services.AddVault(vaultSettings!);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtOptionsConfigurator>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

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
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();
