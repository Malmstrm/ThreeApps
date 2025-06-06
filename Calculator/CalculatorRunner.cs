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
    private void UpdateCalculation()
    {
        Console.Clear();
        Console.WriteLine("Update Calculation");
        Console.WriteLine("------------------");

        var history = _calc.GetHistoryAsync().Result;
        var table = new Table()
            .AddColumns("Id", "Date", "A", "B", "Op", "Result");
        foreach (var c in history)
        {
            table.AddRow(
                c.Id.ToString(),
                c.Date.ToString("yyyy-MM-dd"),
                c.Operand1.ToString("F2"),
                c.Operand2?.ToString("F2") ?? "-",
                c.Operator.ToString(),
                c.Result.ToString("F2")
            );
        }
        AnsiConsole.Write(table);

        Console.WriteLine();
        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of record to update (or type 'cancel'):");
        if (idText.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            return;
        if (!int.TryParse(idText, out int id))
        {
            AnsiConsole.MarkupLine("[red]Invalid Id.[/]");
            Console.ReadKey(true);
            return;
        }

        var existing = _calc.GetByIdAsync(id).Result;
        if (existing is null)
        {
            AnsiConsole.MarkupLine("[red]No record found with that Id.[/]");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine();
        AnsiConsole.MarkupLine("Leave blank to keep existing value.");


        var oldA = existing.Operand1;
        var aStr = AnsiConsole.Prompt(
            new TextPrompt<string>($"First number (current: {oldA:F2}):").AllowEmpty());
        double newA = string.IsNullOrWhiteSpace(aStr) ? oldA : double.Parse(aStr);


        var oldOp = existing.Operator;
        var opChoice = _nav.NavigateWithArrows(
            $"Operator (current: {oldOp}):",
            Enum.GetNames(typeof(CalculatorOperator)));
        var newOp = Enum.Parse<CalculatorOperator>(opChoice);


        double? newB = null;
        if (newOp != CalculatorOperator.SquareRoot)
        {
            var oldB = existing.Operand2 ?? 0;
            var bPrompt = AnsiConsole.Prompt(
                new TextPrompt<string>($"Second number (current: {oldB:F2}):").AllowEmpty());

            newB = string.IsNullOrWhiteSpace(bPrompt)
                ? existing.Operand2
                : double.Parse(bPrompt);
        }


        var cmd = new UpdateCalculationCommand(id, newA, newB, newOp);
        var updated = _calc.UpdateAsync(cmd).Result;

        AnsiConsole.MarkupLine($"\n[green]Record updated! New result: {updated.Result:F2}[/]");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }
}
