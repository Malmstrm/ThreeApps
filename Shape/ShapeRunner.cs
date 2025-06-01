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
            AnsiConsole.Write(new FigletText("Shape Calculator").Centered());

            var choice = _nav.NavigateWithArrows(
                "Choose an option:",
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
                    break;
                case "Update":
                    UpdateCalculation();
                    break;
                case "Delete":
                    DeleteCalculation();
                    break;
                default:
                    return;
            };
        }
    }

    private void PerformCalculation()
    {
        // 1) Välj form
        var shapeStr = _nav.NavigateWithArrows(
            "Select shape:",
            Enum.GetNames(typeof(ShapeType)));
        var shapeType = Enum.Parse<ShapeType>(shapeStr);

        // 2) Läs in parametrar beroende på vald form
        var paramList = new List<ParameterDTO>();
        switch (shapeType)
        {
            case ShapeType.Rectangle:
                {
                    double w = double.Parse(AnsiConsole.Ask<string>("Enter [green]width[/]:"));
                    double h = double.Parse(AnsiConsole.Ask<string>("Enter [green]height[/]:"));
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Width, Value = w });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Parallelogram:
                {
                    double b = double.Parse(AnsiConsole.Ask<string>("Enter [green]base[/]:"));
                    double s = double.Parse(AnsiConsole.Ask<string>("Enter [green]side[/]:"));
                    double h = double.Parse(AnsiConsole.Ask<string>("Enter [green]height[/]:"));
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Base, Value = b });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = s });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Triangle:
                {
                    double a = double.Parse(AnsiConsole.Ask<string>("Enter [green]side A[/]:"));
                    double b = double.Parse(AnsiConsole.Ask<string>("Enter [green]side B[/]:"));
                    double c = double.Parse(AnsiConsole.Ask<string>("Enter [green]side C[/]:"));
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = a });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideB, Value = b });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideC, Value = c });
                }
                break;

            case ShapeType.Rhombus:
                {
                    double d1 = double.Parse(AnsiConsole.Ask<string>("Enter [green]diagonal 1[/]:"));
                    double d2 = double.Parse(AnsiConsole.Ask<string>("Enter [green]diagonal 2[/]:"));
                    double side = double.Parse(AnsiConsole.Ask<string>("Enter [green]side length[/]:"));
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal1, Value = d1 });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal2, Value = d2 });
                    paramList.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = side });
                }
                break;

            default:
                return;
        }

        // 3) Bygg CreateShapeCommand och anropa service
        var cmd = new CreateShapeCommand(shapeType, paramList);
        var dto = _service.CreateAsync(cmd).Result;

        // 4) Visa resultatet
        AnsiConsole.MarkupLine(
            $"\n[bold green]Shape:[/] {dto.ShapeType}  " +
            $"[bold yellow]Area:[/] {dto.Area}  " +
            $"[bold yellow]Perimeter:[/] {dto.Perimeter}");
        Console.ReadKey();
    }
    private void UpdateCalculation()
    {
        // 1) Läs in Id
        var idText = AnsiConsole.Ask<string>("Enter [green]Id[/] of record to update:");
        if (!int.TryParse(idText, out int id))
        {
            AnsiConsole.MarkupLine("[red]Invalid Id.[/]");
            Console.ReadKey();
            return;
        }

        // 2) Hämta befintlig post
        var existing = _service.GetByIdAsyc(id).Result;
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
                    double oldW = existing.Parameters.First(p => p.ParameterType == ParameterType.Width).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var wStr = AnsiConsole.Prompt(
                        new TextPrompt<string>(
                            $"Enter [green]width[/] (default: {oldW}):"
                        ).AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>(
                            $"Enter [green]height[/] (default: {oldH}):"
                        ).AllowEmpty());

                    double w = string.IsNullOrWhiteSpace(wStr) ? oldW : double.Parse(wStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Width, Value = w });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Parallelogram:
                {
                    double oldB = existing.Parameters.First(p => p.ParameterType == ParameterType.Base).Value;
                    double oldS = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldH = existing.Parameters.First(p => p.ParameterType == ParameterType.Height).Value;

                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]base[/] (default: {oldB}):").AllowEmpty());
                    var sStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side[/] (default: {oldS}):").AllowEmpty());
                    var hStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]height[/] (default: {oldH}):").AllowEmpty());

                    double b = string.IsNullOrWhiteSpace(bStr) ? oldB : double.Parse(bStr);
                    double s = string.IsNullOrWhiteSpace(sStr) ? oldS : double.Parse(sStr);
                    double h = string.IsNullOrWhiteSpace(hStr) ? oldH : double.Parse(hStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Base, Value = b });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = s });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Height, Value = h });
                }
                break;

            case ShapeType.Triangle:
                {
                    double oldA = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;
                    double oldB = existing.Parameters.First(p => p.ParameterType == ParameterType.SideB).Value;
                    double oldC = existing.Parameters.First(p => p.ParameterType == ParameterType.SideC).Value;

                    var aStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side A[/] (default: {oldA}):").AllowEmpty());
                    var bStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side B[/] (default: {oldB}):").AllowEmpty());
                    var cStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side C[/] (default: {oldC}):").AllowEmpty());

                    double a = string.IsNullOrWhiteSpace(aStr) ? oldA : double.Parse(aStr);
                    double b = string.IsNullOrWhiteSpace(bStr) ? oldB : double.Parse(bStr);
                    double c = string.IsNullOrWhiteSpace(cStr) ? oldC : double.Parse(cStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = a });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideB, Value = b });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideC, Value = c });
                }
                break;

            case ShapeType.Rhombus:
                {
                    double oldD1 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal1).Value;
                    double oldD2 = existing.Parameters.First(p => p.ParameterType == ParameterType.Diagonal2).Value;
                    double oldS = existing.Parameters.First(p => p.ParameterType == ParameterType.SideA).Value;

                    var d1Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]diagonal 1[/] (default: {oldD1}):").AllowEmpty());
                    var d2Str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]diagonal 2[/] (default: {oldD2}):").AllowEmpty());
                    var sStr = AnsiConsole.Prompt(
                        new TextPrompt<string>($"Enter [green]side length[/] (default: {oldS}):").AllowEmpty());

                    double d1 = string.IsNullOrWhiteSpace(d1Str) ? oldD1 : double.Parse(d1Str);
                    double d2 = string.IsNullOrWhiteSpace(d2Str) ? oldD2 : double.Parse(d2Str);
                    double side = string.IsNullOrWhiteSpace(sStr) ? oldS : double.Parse(sStr);

                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal1, Value = d1 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.Diagonal2, Value = d2 });
                    newParams.Add(new ParameterDTO { ParameterType = ParameterType.SideA, Value = side });
                }
                break;

            default:
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
        Console.ReadKey();
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
}


