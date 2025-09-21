using KitchenStock.Application.Modules.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KitchenStock.Infrastructure.BackgroundServices;

public class TokenCleanupBgService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TokenCleanupBgService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            await refreshTokenRepository.CleanupExpiredTokensAsync();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
