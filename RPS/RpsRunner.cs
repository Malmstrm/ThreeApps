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
            // Hämta historik
            var history = _service.GetHistoryAsync().Result;

            // Bygg och skriv ut tabell
            var table = new Table().AddColumns("Date", "You", "CPU", "Result");
            foreach (var g in history)
            {
                table.AddRow(
                    g.PlayedAt.ToString("yyyy-MM-dd"),
                    g.PlayerMove.ToString(),
                    g.ComputerMove.ToString(),
                    g.Outcome.ToString());
            }
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
