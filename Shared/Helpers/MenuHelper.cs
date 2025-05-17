using Spectre.Console;

namespace Shared.Helpers;

public static class MenuHelper
{
    public static string ShowMenu(string title, params string[] choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold cyan]{title}[/]")
                .AddChoices(choices));
    }

    public static void ShowLoading(string message = "Loading...", int durationMs = 1000)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .Start(message, ctx =>
            {
                Thread.Sleep(durationMs);
            });
    }

    public static void ShowProgressBar(string message = "Processing...", int steps = 100, int delayPerStepMs = 20)
    {
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask($"[cyan]{message}[/]");

                while (!task.IsFinished)
                {
                    task.Increment(100.0 / steps);
                    Thread.Sleep(delayPerStepMs);
                }
            });
    }
}
