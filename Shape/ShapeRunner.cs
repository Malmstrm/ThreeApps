using Application.DTOs;
using Application.Interfaces;
using Application.MappingProfiles;
using Shared.Enums;
using Shared.Interfaces;
using Spectre.Console;

namespace Shape;

public class ShapeRunner : IApp
{
    private readonly INavigationService _nav;
    private readonly IShapeService _service;

    public ShapeRunner(INavigationService nav, IShapeService service)
    {
        _nav = nav;
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("Shape Calculator")
                .Centered()
                .Color(Color.Cyan1));

            var choice = _nav.NavigateWithArrows(
                "Choose an action:",
                "New Calculation",
                "List All",
                "Update",
                "Delete",
                "Return to Main Menu");

            switch (choice)
            {
                case "New Calculation":
                    PerformCalculation();
                    break;
                case "List All":
                    ShowAll();
                    AnsiConsole.MarkupLine("\nPress [green]any key[/] to continue...");
                    Console.ReadKey(true);
                    break;
                case "Update":
                    UpdateCalculation();
                    break;
                case "Delete":
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

        var shapeStr = _nav.NavigateWithArrows(
            "Select shape:",
            Enum.GetNames(typeof(ShapeType)));
        var shapeType = Enum.Parse<ShapeType>(shapeStr);

        var paramList = new List<ParameterDTO>();

        switch (shapeType)
        {
            case ShapeType.Rectangle:
                {
                    AnsiConsole.MarkupLine("Enter [green]width[/] and [green]height[/]:");
                    double w = ReadDouble("Width:");
                    double h = ReadDouble("Height:");
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Width, Value = w });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Parallelogram:
                {
                    AnsiConsole.MarkupLine("Enter [green]side A[/], [green]side B[/] and [green]height[/]:");
                    double sideA = ReadDouble("Side A:");
                    double sideB = ReadDouble("Side B:");
                    double h = ReadDouble("Height:");
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = sideA });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideB, Value = sideB });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Triangle:
                {
                    AnsiConsole.MarkupLine("Enter [green]side A[/], [green]base[/], [green]side C[/] and [green]height[/]:");
                    double sideA = ReadDouble("Side A:");
                    double bas = ReadDouble("Base:");
                    double sideC = ReadDouble("Side C:");
                    double height = ReadDouble("Height:");
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = sideA });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Base, Value = bas });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideC, Value = sideC });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = height });
                }
                break;

            case ShapeType.Rhombus:
                {
                    AnsiConsole.MarkupLine("Enter [green]diagonal 1[/], [green]diagonal 2[/] and [green]side length[/]:");
                    double d1 = ReadDouble("Diagonal 1:");
                    double d2 = ReadDouble("Diagonal 2:");
                    double side = ReadDouble("Side length:");
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal1, Value = d1 });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal2, Value = d2 });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = side });
                }
                break;

            default:
                return;
        }

        var cmd = new CreateShapeCommand(shapeType, paramList);
        var dto = _service.CreateAsync(cmd).Result;

        // 4) Show the result
        AnsiConsole.MarkupLine("\n[bold green]Result:[/]");
        AnsiConsole.MarkupLine($"Shape: [cyan]{dto.ShapeType}[/]");
        AnsiConsole.MarkupLine($"Area: [yellow]{dto.Area:F2}[/]");
        AnsiConsole.MarkupLine($"Perimeter: [yellow]{dto.Perimeter:F2}[/]");

        AnsiConsole.MarkupLine("\nPress [green]any key[/] to continue...");
        Console.ReadKey(true);
    }

    private void UpdateCalculation()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Update Calculation[/]").Centered());
        ShowAll();
        Console.WriteLine();


        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of the record to update:");
        if (!int.TryParse(idText, out int id))
        {
            AnsiConsole.MarkupLine("[red]Invalid Id.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to return...");
            Console.ReadKey(true);
            return;
        }


        var existing = _service.GetByIdAsyc(id).Result;
        if (existing is null)
        {
            AnsiConsole.MarkupLine("[red]No record found with that Id.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to return...");
            Console.ReadKey(true);
            return;
        }

        var shapeType = existing.ShapeType;
        var newParams = new List<ParameterDTO>();

        switch (shapeType)
        {
            case ShapeType.Rectangle:
                {
                    AnsiConsole.MarkupLine("Rectangle – enter new values or press Enter to keep defaults:");
                    double oldW = existing.Parameters.First(p => p.ParameterType == ParameterType.Width).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var wStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Width (default: {oldW}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Height (default: {oldH}):").AllowEmpty());

                    double w = string.IsNullOrWhiteSpace(wStr) ? oldW : double.Parse(wStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Width, Value = w });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Parallelogram:
                {
                    AnsiConsole.MarkupLine("Parallelogram – enter new values or press Enter to keep defaults:");
                    double oldA = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldB = existing.Parameters.First(p => p.ParameterType == ParameterType.SideB).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var aStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Side A (default: {oldA}):").AllowEmpty());
                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Side B (default: {oldB}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Height (default: {oldH}):").AllowEmpty());

                    double sideA = string.IsNullOrWhiteSpace(aStr) ? oldA : double.Parse(aStr);
                    double sideB = string.IsNullOrWhiteSpace(bStr) ? oldB : double.Parse(bStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = sideA });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideB, Value = sideB });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Triangle:
                {
                    AnsiConsole.MarkupLine("Triangle – enter new values or press Enter to keep defaults:");
                    double oldA = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldB = existing.Parameters.First(p => p.ParameterType == ParameterType.Base).Value;
                    double oldC = existing.Parameters.First(p => p.ParameterType == ParameterType.SideC).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var aStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Side A (default: {oldA}):").AllowEmpty());
                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Base   (default: {oldB}):").AllowEmpty());
                    var cStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Side C (default: {oldC}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Height (default: {oldH}):").AllowEmpty());

                    double sideA = string.IsNullOrWhiteSpace(aStr) ? oldA : double.Parse(aStr);
                    double bas = string.IsNullOrWhiteSpace(bStr) ? oldB : double.Parse(bStr);
                    double sideC = string.IsNullOrWhiteSpace(cStr) ? oldC : double.Parse(cStr);
                    double height = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = sideA });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Base, Value = bas });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideC, Value = sideC });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = height });
                }
                break;

            case ShapeType.Rhombus:
                {
                    AnsiConsole.MarkupLine("Rhombus – enter new values or press Enter to keep defaults:");
                    double oldD1 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal1).Value;
                    double oldD2 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal2).Value;
                    double oldSide = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;

                    var d1Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Diagonal 1 (default: {oldD1}):").AllowEmpty());
                    var d2Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Diagonal 2 (default: {oldD2}):").AllowEmpty());
                    var sStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Side length (default: {oldSide}):").AllowEmpty());

                    double d1 = string.IsNullOrWhiteSpace(d1Str) ? oldD1 : double.Parse(d1Str);
                    double d2 = string.IsNullOrWhiteSpace(d2Str) ? oldD2 : double.Parse(d2Str);
                    double side = string.IsNullOrWhiteSpace(sStr) ? oldSide : double.Parse(sStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal1, Value = d1 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal2, Value = d2 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = side });
                }
                break;

            default:
                return;
        }

        var updateCmd = new UpdateShapeCommand(id, shapeType, newParams);
        _service.UpdateAsync(updateCmd).Wait();

        AnsiConsole.MarkupLine("\n[green]Record updated![/]");
        AnsiConsole.MarkupLine("Press [green]any key[/] to continue...");
        Console.ReadKey(true);
    }

    private void ShowAll()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]All Calculations[/]").Centered());

        var list = _service.GetAllAsync().Result;

        var table = new Table()
            .BorderColor(Color.Grey37)
            .AddColumns("Id", "Date", "Shape", "Area", "Perimeter", "Parameters");

        foreach (var s in list)
        {
            string paramsText = string.Join("; ",
                s.Parameters.Select(p => $"{p.ParameterType}={p.Value}"));

            table.AddRow(
                s.Id.ToString(),
                s.Date.ToString("yyyy-MM-dd"),
                s.ShapeType.ToString(),
                s.Area.ToString("F2"),
                s.Perimeter.ToString("F2"),
                paramsText
            );
        }

        AnsiConsole.Write(table);
    }
    private void DeleteCalculation()
    {
        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of record to delete:");
        if (!int.TryParse(idText, out int id))
        {
            AnsiConsole.MarkupLine("[red]Invalid Id.[/]");
            Console.ReadKey();
            return;
        }

        _service.DeleteAsync(id).Wait();
        AnsiConsole.MarkupLine("[yellow]Record deleted![/]");
        Console.ReadKey();
    }
    private double ReadDouble(string prompt)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>($"{prompt}");
            if (double.TryParse(input, out var val))
                return val;

            AnsiConsole.MarkupLine("[red]Invalid number – please try again.[/]");
        }
    }
}


