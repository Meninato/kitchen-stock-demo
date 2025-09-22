using KitchenStock.Application.Modules.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KitchenStock.Infrastructure.BackgroundServices;

public class TokenCleanupBgService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TokenCleanupBgService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            await refreshTokenRepository.CleanupExpiredTokensAsync();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
