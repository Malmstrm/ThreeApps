using Autofac;
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

            // Menu Selection
            var selectedApp = _navigation.NavigateWithArrows(
                "Select an option:",
                "🎮  Rock, Paper & Scissors",
                "📐  Shape Calculator",
                "🧮  Calculator",
                "❌  Exit"
            );

            if (selectedApp.Contains("Exit"))
            {
                AnsiConsole.MarkupLine("\n[green]Exiting application. Goodbye![/]");
                AnsiConsole.Write(new Rule("[yellow]Thank you for using ThreeApps![/]").Centered());
                Thread.Sleep(1000); // Liten delay för att användaren ska hinna se det
                break;
            }

            // Visual Feedback
            AnsiConsole.MarkupLine($"\n[bold green]Launching {selectedApp}...[/]");
            Thread.Sleep(700);

            var appKey = selectedApp.ToLower() switch
            {
                var s when s.Contains("rock") => "rps",
                var s when s.Contains("shape") => "shape",
                var s when s.Contains("calculator") => "calculator",
                _ => null
            };

            if (appKey != null && _container.IsRegisteredWithName<IApp>(appKey))
            {
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
