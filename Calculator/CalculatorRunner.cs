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
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]New Calculation[/]").Centered());


        if (!TryReadDouble("If you want to Cancel, just type 'cancel'\nEnter first number:", out double a))
            return; //


        var opStr = _nav.NavigateWithArrows(
            "Operator:",
            "+",
            "-",
            "*",
            "/",
            "√",
            "%");

        if (opStr == null)
            return; 


        CalculatorOperator op = opStr switch
        {
            "+" => CalculatorOperator.Addition,
            "-" => CalculatorOperator.Subtraction,
            "*" => CalculatorOperator.Multiplication,
            "/" => CalculatorOperator.Division,
            "√" => CalculatorOperator.SquareRoot,
            "%" => CalculatorOperator.Modulus,
            _ => throw new InvalidOperationException("Unknown operator")
        };


        double? b = null;
        if (op != CalculatorOperator.SquareRoot)
        {
            while (true)
            {
                if (!TryReadDouble("Enter second number (or type 'cancel'):", out double tmp))
                {
                    return;
                }

                if ((op == CalculatorOperator.Division || op == CalculatorOperator.Modulus) && tmp == 0)
                {
                    AnsiConsole.MarkupLine("[red]Cannot divide (or take modulus) by zero. Please enter a non‐zero number.[/]");
                    continue; 
                }

                b = tmp;
                break;
            }
        }


        var cmd = new CalculateCommand(a, b, op);
        var res = _calc.CalculationAsync(cmd).Result;


        AnsiConsole.MarkupLine($"\nResult: [bold yellow]{res.Result:F2}[/]");


        AnsiConsole.MarkupLine("\nPress [green]Y[/] to do another calculation, or [red]Esc[/] to return to calculator menu");
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Y)
            {
                PerformCalculation(); 
                return;
            }
            else if (key == ConsoleKey.Escape)
            {
                return; 
            }

        }
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
        Console.WriteLine();


        var history = _calc.GetHistoryAsync().Result;
        if (!history.Any())
        {
            Console.WriteLine("No calculations in history.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

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

        var idText = AnsiConsole.Ask<string>("Enter Id of record to update (or type 'cancel'):");
        if (idText.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            return;

        if (!int.TryParse(idText, out int id))
        {
            Console.WriteLine("Invalid Id.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        var existing = _calc.GetByIdAsync(id).Result;
        if (existing is null)
        {
            Console.WriteLine($"No record found with Id {id}.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Leave input blank to keep existing value.");
        Console.WriteLine("Type 'cancel' to abort and return to menu.");
        Console.WriteLine();


        var oldA = existing.Operand1;
        if (!TryReadDouble($"First number (current: {oldA:F2}):", out double newA, oldA))
            return; 


        var opNames = Enum.GetNames(typeof(CalculatorOperator));
        var opChoice = _nav.NavigateWithArrows($"Operator (current: {existing.Operator}):", opNames);
        if (opChoice == null)
            return;

        var newOp = Enum.Parse<CalculatorOperator>(opChoice);


        double? newB = null;
        if (newOp != CalculatorOperator.SquareRoot)
        {
            var oldB = existing.Operand2 ?? 0.0;
            while (true)
            {

                if (!TryReadDouble($"Second number (current: {oldB:F2}):", out double tmp, oldB))
                    return;


                if ((newOp == CalculatorOperator.Division || newOp == CalculatorOperator.Modulus) && tmp == 0)
                {
                    Console.WriteLine("Cannot divide or modulus by zero. Please enter a non-zero number (or 'cancel').");
                    continue;
                }

                newB = tmp;
                break;
            }
        }


        var cmd = new UpdateCalculationCommand(id, newA, newB, newOp);
        var updated = _calc.UpdateAsync(cmd).Result;

        Console.WriteLine();
        Console.WriteLine($"Record updated! New result: {updated.Result:F2}");
        Console.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }

    private void DeleteCalculation()
    {
        Console.Clear();
        Console.WriteLine("Delete Calculation");
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
        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of record to delete (or type 'cancel'):");
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

        bool confirm = AnsiConsole.Confirm($"Are you sure you want to delete Id {id} (result={existing.Result:F2})?");
        if (!confirm)
        {
            AnsiConsole.MarkupLine("Deletion canceled. Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        _calc.DeleteAsync(id).Wait();
        AnsiConsole.MarkupLine("\n[red]Record deleted![/]");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
    private bool TryReadDouble(string prompt, out double value, double? defaultValue = null)
    {
        while (true)
        {

            var displayPrompt = defaultValue.HasValue
                ? $"{prompt} (default: {defaultValue.Value:F2})"
                : prompt;

            var input = AnsiConsole.Prompt(new TextPrompt<string>(displayPrompt).AllowEmpty());


            if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
            {
                value = defaultValue.Value;
                return true;
            }


            if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                value = 0;
                return false;
            }


            if (double.TryParse(input, out value))
            {
                return true;
            }


            Console.WriteLine("Invalid number – please try again or type 'cancel' to abort.");
        }
    }
}