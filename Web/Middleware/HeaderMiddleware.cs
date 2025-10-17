using Business.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Middleware
{
    public class HeaderMiddleware(
        RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory,
        IMemoryCache memoryCache)
    {
        private const string CacheKey = "TotalGamesCount";

        public async Task InvokeAsync(HttpContext context)
        {
            if (!memoryCache.TryGetValue(CacheKey, out int totalGamesCount))
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                    var games = await gameService.GetAllGamesAsync();
                    totalGamesCount = games.Count();
                }

                memoryCache.Set(CacheKey, totalGamesCount, TimeSpan.FromMinutes(1));
            }
            context.Response.Headers["x-total-numbers-of-games"] = totalGamesCount.ToString();

            await next(context);
        }
    }
}