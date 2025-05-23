using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependencies;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRps(this IServiceCollection services)
    {
        services.AddScoped<IRpsRepository, RpsRepository>();
        services.AddScoped<IRpsService, RpsService>();
        return services;
    }
}
