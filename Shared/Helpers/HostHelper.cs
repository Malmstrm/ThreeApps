using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

namespace Shared.Helpers;

public static class HostHelper
{
    public static IApp BuildAndRun<TApp>() where TApp : class, IApp
    {
        var host = Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                // Registrera NavigationService + Spectre.Console helpers
                builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
                builder.RegisterType<TApp>().As<IApp>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IApp>();
    }
}
