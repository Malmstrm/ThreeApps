using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
using Shared.Services;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MainMenu;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            // Byt ut MS-DI mot Autofac som kontainer
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())

                .ConfigureLogging(logging =>
                {
                    // Behåll gärna konsolloggern för din egen kod:
                    logging.ClearProviders();
                    logging.AddConsole();

                    // Tystar all EF Core-loggning:
                    logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
                })

            // 1) Registrera "framework"-tjänster i MS-DI
            .ConfigureServices((ctx, services) =>
            {
                // a) EF Core: DbContext + anslutning
                var connString = ctx.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseSqlServer(connString));

                // b) AutoMapper: scanna alla mapping-profiler i Core.Application
                services.AddAutoMapper(typeof(Application.MappingProfiles.RpsMappingProfile).Assembly);
            })

            // 2) Registrera dina egna implementationer i Autofac
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                // a) NavigationService från Shared
                builder
                    .RegisterType<NavigationService>()
                    .As<INavigationService>()
                    .SingleInstance();

                // b) RPS: Repository + Service
                builder
                    .RegisterType<RpsRepository>()
                    .As<IRpsRepository>()
                    .InstancePerLifetimeScope();
                builder
                    .RegisterType<RpsService>()
                    .As<IRpsService>()
                    .InstancePerLifetimeScope();

                // c) Console-apparna själva
                builder.RegisterType<MenuRunner>().AsSelf();
                builder.RegisterType<RPS.RpsRunner>().AsSelf();
                // (du kan även registrera ShapeRunner, CalcRunner etc här)
            })
            .Build();
        using (var migScope = host.Services.CreateScope())
        {
            var db = migScope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Skapa scope och kör huvudmenyn
        using var scope = host.Services.CreateScope();
        var menu = scope.ServiceProvider.GetRequiredService<MenuRunner>();
        menu.Run();
    }
}
