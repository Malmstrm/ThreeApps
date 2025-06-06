using Application.DTOs;
using Application.Interfaces;
using Shared.Enums;
using Shared.Interfaces;
using Spectre.Console;

namespace Calculator;

public class CalculatorRunner
{
    private readonly INavigationService _nav;
    private readonly ICalculatorService _calc;

    public CalculatorRunner(INavigationService nav, ICalculatorService calc)
    {
        _nav = nav;
        _calc = calc;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("Calculator").Centered());


            var choice = _nav.NavigateWithArrows(
                "Choose an option:",
                "New Calculation",
                "List All",
                "Update Calculation",
                "Delete Calculation",
                "Return to Main Menu"
            );


            if (choice == null || choice == "Return to Main Menu")
            {
                return;
            }


            switch (choice)
            {
                case "New Calculation":
                    PerformCalculation();
                    break;

                case "List All":
                    ShowHistory();
                    break;

                case "Update Calculation":
                    UpdateCalculation();
                    break;

                case "Delete Calculation":
                    DeleteCalculation();
                    break;

                default:

                    return;
            }
        }
    }
    private void PerformCalculation()
    {
        // 1) Läs in tal1
        var a = double.Parse(AnsiConsole.Ask<string>("Enter [green]first[/] number:"));

        // 2) Välj operator
        var op = _nav.NavigateWithArrows(
            "Operator:",
            Enum.GetNames(typeof(CalculatorOperator)));

        // 3) Läs in tal2 (om ej √)
        double? b = null;
        if (Enum.Parse<CalculatorOperator>(op) != CalculatorOperator.SquareRoot)
            b = double.Parse(AnsiConsole.Ask<string>("Enter [green]second[/] number:"));

        // 4) Räkna och visa
        var cmd = new CalculateCommand(a, b, Enum.Parse<CalculatorOperator>(op));
        var res = _calc.CalculationAsync(cmd).Result;

        AnsiConsole.MarkupLine($"\nResult: [bold yellow]{res.Result}[/]");
        Console.ReadKey();
    }
    private void ShowHistory()
    {
        var history = _calc.GetHistoryAsync().Result;

        var table = new Table()
            .AddColumns("Date", "A", "B", "Op", "Result");

        foreach (var c in history)
            table.AddRow(
                c.Date.ToString("d"),
                c.Operand1.ToString(),
                c.Operand2?.ToString() ?? "-",
                c.Operator.ToString(),
                c.Result.ToString("F2")
            );

        AnsiConsole.Write(table);
        Console.ReadKey();
    }
}
