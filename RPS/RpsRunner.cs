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

                if (choice == "Play")
                    PlayRound();
                else if (choice == "History")
                    ShowHistory();
                else
                    break;
            }
        }

        private void PlayRound()
        {
            // 1) Låt användaren välja drag
            var moveStr = _navigation.NavigateWithArrows(
                "Your move:", "Rock", "Paper", "Scissors");
            var cmd = new PlayRpsCommand(Enum.Parse<RPSMove>(moveStr));

            // 2) Spela via IRpsService
            var result = _service.PlayAsync(cmd).Result;

            // 3) Visa utfall och procent
            AnsiConsole.MarkupLine($"\nYou: [yellow]{result.PlayerMove}[/]" +
                                   $"  CPU: [yellow]{result.ComputerMove}[/]" +
                                   $"  → [bold cyan]{result.Outcome}![/]");

            var winRate = _service.GetWinRateAsync().Result;
            AnsiConsole.MarkupLine($"[grey]Win-rate overall: {winRate:P1}[/]");

            Console.ReadKey();
        }

        private void ShowHistory()
        {
            // 1) Hämta och sortera historiken kronologiskt
            var history = _service.GetHistoryAsync().Result
                                  .OrderBy(g => g.PlayedAt)
                                  .ToList();

            // 2) Lägg till en extra kolumn "Games" före win%-kolumnen
            var table = new Table()
                .AddColumns("Date", "You", "CPU", "Result", "Games", "Win %");

            int total = 0;
            int wins = 0;

            // 3) Fyll på rad-för-rad
            foreach (var g in history)
            {
                total++;
                if (g.Outcome == GameOutcome.Win)
                    wins++;

                double rate = (double)wins / total;

                table.AddRow(
                    g.PlayedAt.ToString("d"),
                    g.PlayerMove.ToString(),
                    g.ComputerMove.ToString(),
                    g.Outcome.ToString(),
                    total.ToString(),         // <-- Antal spel hittills
                    rate.ToString("P1")       // <-- Kumulativ win%
                );
            }

            // 4) Rendera tabellen
            AnsiConsole.Write(table);
            Console.ReadKey();
        }



        private static void ShowHeader() =>
            AnsiConsole.Write(
                new FigletText("Rock Paper Scissors")
                  .Centered()
                  .Color(Color.Cyan1)
            );
    }
}
