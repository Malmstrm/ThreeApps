using Application.DTOs;
using Application.Interfaces;
using Shared.Enums;
using Shared.Interfaces;
using Spectre.Console;
using System;
using System.Linq;

namespace RPS
{
    public class RpsRunner
    {
        private readonly INavigationService _navigation;
        private readonly IRpsService _service;

        public RpsRunner(
            INavigationService navigation,
            IRpsService service)
        {
            _navigation = navigation;
            _service = service;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                ShowHeader();

                var choice = _navigation.NavigateWithArrows(
                    "Choose an option:",
                    "Play",
                    "History",
                    "Return to Main Menu");

                if (choice == null || choice == "Return to Main Menu")
                {
                    break;
                }

                if (choice == "Play")
                    PlayRound();
                else if (choice == "History")
                    ShowHistory();
            }
        }


        private void PlayRound()
        {
            Console.Clear();
            ShowHeader();


            var moveStr = _navigation.NavigateWithArrows(
                "Your move (press Esc to cancel):",
                "Rock",
                "Paper",
                "Scissors");

            if (moveStr == null)
            {

                return;
            }

            var playerMove = Enum.Parse<RPSMove>(moveStr);
            AnsiConsole.MarkupLine($"\nYou chose: [yellow]{playerMove}[/]");


            AnsiConsole.Status().Start("CPU is thinking...", ctx => Thread.Sleep(700));


            var played = _service.PlayAsync(new PlayRpsCommand(playerMove)).Result;


            AnsiConsole.MarkupLine($"\nYou: [yellow]{played.PlayerMove}[/]" +
                                   $"  CPU: [yellow]{played.ComputerMove}[/]" +
                                   $"  → [bold cyan]{played.Outcome}![/]");


            Console.WriteLine();
            AnsiConsole.MarkupLine("Play again? [green]Y[/] or [red]N[/] (or press Esc to return)");
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                {
                    PlayRound();
                    return;
                }
                else if (key == ConsoleKey.N || key == ConsoleKey.Escape)
                {
                    return;
                }
            }
        }

        private void ShowHistory()
        {
            Console.Clear();
            ShowHeader();

            AnsiConsole.MarkupLine("\nPress [yellow]Esc[/] to cancel, or any other key to view history...");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape)
                return;

            var history = _service.GetHistoryAsync().Result
                                  .OrderByDescending(g => g.PlayedAt)
                                  .ToList();

            if (!history.Any())
            {
                AnsiConsole.MarkupLine("\n[grey]No games played yet.[/]");
                AnsiConsole.MarkupLine("\nPress [green]any key[/] to return...");
                Console.ReadKey(true);
                return;
            }

            var table = new Table()
                .AddColumns("Date", "You", "CPU", "Result", "Games", "Wins", "Losses", "Ties", "Win %");

            foreach (var g in history)
            {
                double winRate = 0;
                if (g.Games > 0)
                    winRate = g.Wins / (double)g.Games;

                table.AddRow(
                    g.PlayedAt.ToString("yyyy-MM-dd"),
                    g.PlayerMove.ToString(),
                    g.ComputerMove.ToString(),
                    g.Outcome.ToString(),
                    g.Games.ToString(),
                    g.Wins.ToString(),
                    g.Losses.ToString(),
                    g.Ties.ToString(),
                    winRate.ToString("P1")
                );
            }

            AnsiConsole.Write(table);
            Console.WriteLine();
            AnsiConsole.MarkupLine("Press [green]any key[/] to return...");
            Console.ReadKey(true);
        }

        private static void ShowHeader() =>
            AnsiConsole.Write(
                new FigletText("Rock Paper Scissors")
                  .Centered()
                  .Color(Color.Cyan1)
            );
    }
}
