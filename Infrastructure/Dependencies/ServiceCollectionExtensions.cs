using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependencies;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRps(this IServiceCollection services)
    {
        services.AddTransient<IRpsRepository, RpsRepository>();
        services.AddTransient<IRpsService, RpsService>();
        services.AddTransient<ICalculatorRepository, CalculatorRepository>();
        services.AddTransient<ICalculatorService, CalculationService>();
        return services;
    }
}
