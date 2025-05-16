using Application.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        //builder.RegisterType<ShapeApp.ShapeApplication>().Named<IApp>("shape");
        //builder.RegisterType<Calculator.CalculatorApplication>().Named<IApp>("calculator");
        //builder.RegisterType<RPS.RpsApplication>().Named<IApp>("rps");
    })
    .Build();

using var scope = host.Services.CreateScope();
var container = (ILifetimeScope)scope.ServiceProvider.GetService(typeof(ILifetimeScope));

// Enkelt menyval i Console
Console.WriteLine("Vilken app vill du köra? (shape/calculator/rps)");
var input = Console.ReadLine()?.ToLower();

if (!string.IsNullOrWhiteSpace(input) && container.IsRegisteredWithName<IApp>(input))
{
    var app = container.ResolveNamed<IApp>(input);
    app.Run();
}
else
{
    Console.WriteLine("Ogiltigt val, program avslutas.");
}