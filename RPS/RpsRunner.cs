using Shared.Interfaces;
using Spectre.Console;

namespace RPS;

public class RpsRunner
{
    private readonly INavigationService _navigation;

    public RpsRunner(INavigationService navigation)
    {
        _navigation = navigation;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();

            // Header
            AnsiConsole.Write(new FigletText("Rock Paper Scissors")
                .Centered()
                .Color(Color.Cyan1));

            AnsiConsole.Write(new Panel("[bold yellow]Welcome to RPS Game![/]")
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(Color.Cyan1))
                .Header("[bold cyan]RPS Menu[/]")
                .Padding(1, 1)
                .Expand());

            var choice = _navigation.NavigateWithArrows(
                "Choose an option:",
                "Play",
                "History",
                "Return to Main Menu"
            );

            if (choice == "Play")
            {
                AnsiConsole.MarkupLine("\n[bold green]Starting the game...[/]");
                // Här kommer du anropa RpsApplication senare
                AnsiConsole.MarkupLine("[italic yellow](Game logic not implemented yet)[/]");
                Thread.Sleep(1000);
            }
            else if (choice == "History")
            {
                AnsiConsole.MarkupLine("\n[bold cyan]Showing game history...[/]");
                // Här kommer du anropa RpsHistory senare
                AnsiConsole.MarkupLine("[italic yellow](History view not implemented yet)[/]");
                Thread.Sleep(1000);
            }
            else if (choice == "Return to Main Menu")
            {
                AnsiConsole.MarkupLine("\n[green]Returning to Main Menu...[/]");
                Thread.Sleep(500);
                break;
            }
        }
    }
}
