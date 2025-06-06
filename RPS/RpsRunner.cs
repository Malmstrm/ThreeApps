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

            Console.WriteLine();
            Console.WriteLine("Press [Esc] to cancel, or any other key to view history...");
            var initialKey = Console.ReadKey(true).Key;
            if (initialKey == ConsoleKey.Escape)
                return;

            var history = _service.GetHistoryAsync().Result
                                  .OrderByDescending(g => g.PlayedAt)
                                  .ToList();

            if (!history.Any())
            {
                Console.WriteLine();
                Console.WriteLine("No games played yet.");
                Console.WriteLine();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                return;
            }

            const int pageSize = 10;
            int totalItems = history.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            int currentPage = 0;

            while (true)
            {
                Console.Clear();
                ShowHeader();

                Console.WriteLine();
                Console.WriteLine($"Page {currentPage + 1} of {totalPages}");
                Console.WriteLine();

                var table = new Table()
                    .AddColumns("Date", "You", "CPU", "Result", "Games", "Wins", "Losses", "Ties", "Win %");

                var pageItems = history
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);

                foreach (var g in pageItems)
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
                Console.WriteLine("Left arrow, Previous page   Right arrow, Next page   Esc Return");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                    return;
                else if (key == ConsoleKey.RightArrow && currentPage < totalPages - 1)
                    currentPage++;
                else if (key == ConsoleKey.LeftArrow && currentPage > 0)
                    currentPage--;
            }
        }

        private static void ShowHeader() =>
            AnsiConsole.Write(
                new FigletText("Rock Paper Scissors")
                  .Centered()
                  .Color(Color.Cyan1)
            );
    }
}
