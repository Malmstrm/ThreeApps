using Application.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;
using Shared.Services;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        // Register Shared Services
        builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

        // Register Apps
        //builder.RegisterType<ShapeApplication>().Named<IApp>("shape");
        //builder.RegisterType<CalculatorApplication>().Named<IApp>("calculator");
        //builder.RegisterType<RpsApplication>().Named<IApp>("rps");
    })
    .Build();

using var scope = host.Services.CreateScope();
var container = (ILifetimeScope)scope.ServiceProvider.GetService(typeof(ILifetimeScope));

// Get Navigation Service
var navigation = container.Resolve<INavigationService>();

// Display Main Menu with Arrows
var selectedApp = navigation.NavigateWithArrows(
    "Välj vilken app du vill köra:",
    "shape",
    "calculator",
    "rps"
);

// Run the selected app
//if (container.IsRegisteredWithName<IApp>(selectedApp))
//{
//    var app = container.ResolveNamed<IApp>(selectedApp);
//    app.Run();
//}
//else
//{
//    Console.WriteLine("Ogiltigt val, program avslutas.");
//}
