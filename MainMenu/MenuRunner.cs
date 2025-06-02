using Autofac;
using Shared.Helpers;
using Shared.Interfaces;
using Spectre.Console;

namespace MainMenu;

public class MenuRunner
{
    private readonly ILifetimeScope _container;
    private readonly INavigationService _navigation;

    public MenuRunner(ILifetimeScope container, INavigationService navigation)
    {
        _container = container;
        _navigation = navigation;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();

            // Header
            AnsiConsole.Write(new FigletText("Main Menu")
                .Centered()
                .Color(Color.Cyan1));

            AnsiConsole.Write(new Panel("[bold yellow]Welcome to ThreeApps Suite![/]")
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(Color.Cyan1))
                .Header("[bold cyan]Navigation[/]")
                .Padding(1, 1)
                .Expand());

            // Menu Selection (Plain Text)
            var selectedApp = _navigation.NavigateWithArrows(
                "Select an option:",
                "RPS - Rock, Paper & Scissors",
                "Shape - Shape Calculator",
                "Calc - Calculator",
                "Exit - Exit"
            );

            if (selectedApp.Contains("Exit"))
            {
                AnsiConsole.MarkupLine("\n[green]Exiting application. Goodbye![/]");
                AnsiConsole.Write(new Rule("[yellow]Thank you for using ThreeApps![/]").Centered());
                Thread.Sleep(1000);
                break;
            }


            // Determine App Key and Loading Time
            var (appKey, loadingTime) = selectedApp.ToLower() switch
            {
                var s when s.Contains("rps") => ("rps", 1000),
                var s when s.Contains("shape") => ("shape", 1500),
                var s when s.Contains("calc") => ("calc", 2000),
                _ => (null, 1000)
            };

            if (appKey == "rps")
            {
                AnsiConsole.MarkupLine($"\n[bold green]Launching {selectedApp}...[/]");
                MenuHelper.ShowLoading("Loading application...", loadingTime);

                var rpsRunner = _container.Resolve<RPS.RpsRunner>();
                rpsRunner.Run();
            }
            else if (appKey == "calc")
            {
                AnsiConsole.MarkupLine($"\n[bold green]Launching {selectedApp}...[/]");
                MenuHelper.ShowLoading("Loading application...", loadingTime);

                var calcRunner = _container.Resolve<Calculator.CalculatorRunner>();
                calcRunner.Run();
            }
            else if (appKey == "shape")
            {
                AnsiConsole.MarkupLine($"\n[bold green]Launching {selectedApp}...[/]");
                MenuHelper.ShowLoading("Loading application...", loadingTime);

                var shapeRunner = _container.Resolve<Shape.ShapeRunner>();
                shapeRunner.Run();
            }
            else if (appKey != null && _container.IsRegisteredWithName<IApp>(appKey))
            {
                AnsiConsole.MarkupLine($"\n[bold green]Launching {selectedApp}...[/]");
                MenuHelper.ShowLoading("Loading application...", loadingTime);

                var app = _container.ResolveNamed<IApp>(appKey);
                app.Run();
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid selection. Try again.[/]");
                Thread.Sleep(1000);
            }
        }
    }
}
