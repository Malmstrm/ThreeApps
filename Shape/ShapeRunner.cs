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
        ShowAll();

        // 1) Läs in Id
        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of record to update:");
        if (!int.TryParse(idText, out int id))
        {
            AnsiConsole.MarkupLine("[red]Invalid Id.[/]");
            return;
        }

        // 2) Hämta befintlig post
        var existing = _service.GetByIdAsyc(id).Result;  // Förutsätter att metoden heter GetByIdAsync
        if (existing is null)
        {
            AnsiConsole.MarkupLine("[red]No record found with that Id.[/]");
            Console.ReadKey();
            return;
        }

        // 3) Förifyll befintliga värden och låt användaren ändra eller trycka Enter
        var shapeType = existing.ShapeType;
        var newParams = new List<ParameterDTO>();

        switch (shapeType)
        {
            case ShapeType.Rectangle:
                {
                    // Rektangel: Width + Height
                    double oldW = existing.Parameters.First(p => p.ParameterType == ParameterType.Width).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    // Använd TextPrompt<string> med .AllowEmpty() för att ge förifylld inmatning
                    var wStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]width[/] (default: {oldW}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]height[/] (default: {oldH}):").AllowEmpty());

                    double w = string.IsNullOrWhiteSpace(wStr) ? oldW : double.Parse(wStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Width, Value = w });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Parallelogram:
                {
                    // Parallelogram: SideA + SideB + Height
                    // (Tidigare Base + SideA + Height, men vi vill nu använda två sidor istället för Base och SideA)
                    // Antag att din parallellogram‐kod räknar area = SideA × Height och peri = 2×(SideA + SideB).
                    double oldSideA = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldSideB = existing.Parameters.First(p => p.ParameterType == ParameterType.SideB).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var aStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side A[/] (default: {oldSideA}):").AllowEmpty());
                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side B[/] (default: {oldSideB}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]height[/] (default: {oldH}):").AllowEmpty());

                    double sideA = string.IsNullOrWhiteSpace(aStr) ? oldSideA : double.Parse(aStr);
                    double sideB = string.IsNullOrWhiteSpace(bStr) ? oldSideB : double.Parse(bStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = sideA });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideB, Value = sideB });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Triangle:
                {
                    // Triangel: SideA + Base + SideC + Height
                    double oldA = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldB = existing.Parameters.First(p => p.ParameterType == ParameterType.Base).Value;
                    double oldC = existing.Parameters.First(p => p.ParameterType == ParameterType.SideC).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var aStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side A[/] (default: {oldA}):").AllowEmpty());
                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]base[/] (default: {oldB}):").AllowEmpty());
                    var cStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side C[/] (default: {oldC}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]height[/] (default: {oldH}):").AllowEmpty());

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
                    // Rhombus: Diagonal1 + Diagonal2 + SideA
                    double oldD1 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal1).Value;
                    double oldD2 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal2).Value;
                    double oldSide = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;

                    var d1Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]diagonal 1[/] (default: {oldD1}):").AllowEmpty());
                    var d2Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]diagonal 2[/] (default: {oldD2}):").AllowEmpty());
                    var sStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side length[/] (default: {oldSide}):").AllowEmpty());

                    double d1 = string.IsNullOrWhiteSpace(d1Str) ? oldD1 : double.Parse(d1Str);
                    double d2 = string.IsNullOrWhiteSpace(d2Str) ? oldD2 : double.Parse(d2Str);
                    double side = string.IsNullOrWhiteSpace(sStr) ? oldSide : double.Parse(sStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal1, Value = d1 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal2, Value = d2 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = side });
                }
                break;

            default:
                // Om vi av misstag kommer hit, går vi tillbaka till huvudmenyn
                return;
        }

        // 4) Skicka UpdateShapeCommand till service
        var updateCmd = new UpdateShapeCommand(id, shapeType, newParams);
        _service.UpdateAsync(updateCmd).Wait();

        AnsiConsole.MarkupLine("[green]Record updated![/]");
        Console.ReadKey();
    }

    private void ShowAll()
    {
        var list = _service.GetAllAsync().Result;

        var table = new Table()
            .AddColumns("Id", "Date", "Shape", "Area", "Perimeter", "Parameters");

        foreach (var s in list)
        {
            string paramsText = string.Join("; ",
                s.Parameters.Select(p => $"{p.ParameterType}={p.Value}"));

            table.AddRow(
                s.Id.ToString(),
                s.Date.ToString("d"),
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


