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
            AnsiConsole.Write(new FigletText("Main Menu").Centered().Color(Color.Cyan1));

            var selectedApp = _navigation.NavigateWithArrows(
                "Select an option:",
                "Rock, Paper & Scissors",
                "Shape Calculator",
                "Calculator",
                "Exit"
            );

            if (selectedApp == "Exit")
            {
                AnsiConsole.MarkupLine("[green]Exiting application. Goodbye![/]");
                break;
            }

            var appKey = selectedApp.ToLower() switch
            {
                "rock, paper & scissors" => "rps",
                "shape calculator" => "shape",
                "calculator" => "calculator",
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
            }
        }
    }

}
