using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;
using Shared.Services;
using MainMenu;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

        //builder.RegisterType<ShapeApplication>().Named<IApp>("shape");
        //builder.RegisterType<CalculatorApplication>().Named<IApp>("calculator");
        //builder.RegisterType<RpsApplication>().Named<IApp>("rps");

        // Registrera MenuRunner i DI
        builder.RegisterType<MenuRunner>().AsSelf();
        builder.RegisterType<RPS.RpsRunner>().AsSelf();
    })
    .Build();

using var scope = host.Services.CreateScope();
var container = (ILifetimeScope)scope.ServiceProvider.GetService(typeof(ILifetimeScope));

// Hämta MenuRunner via DI och kör
var menu = container.Resolve<MenuRunner>();
menu.Run();