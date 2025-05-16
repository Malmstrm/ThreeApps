using Shared.Interfaces;
using Spectre.Console;
using System;


namespace Shared.Services;

public class NavigationService : INavigationService
{
    public string NavigateWithArrows(string title, params string[] options)
    {
        int index = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[bold cyan]{title}[/]\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == index)
                    AnsiConsole.MarkupLine($"[bold yellow]> {options[i]}[/]");
                else
                    AnsiConsole.MarkupLine($"  {options[i]}");
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                index = (index == 0) ? options.Length - 1 : index - 1;
            else if (key == ConsoleKey.DownArrow)
                index = (index + 1) % options.Length;

        } while (key != ConsoleKey.Enter);

        return options[index];
    }
}
