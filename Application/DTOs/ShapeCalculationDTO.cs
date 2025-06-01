using Shared.Enums;

namespace Application.DTOs;

public class ShapeCalculationDTO
{
    public int Id { get; init; }
    public ShapeType ShapeType { get; init; }
    public double Area { get; init; }
    public double Perimeter { get; init; }
    public DateOnly Date { get; init; }
    public IReadOnlyList<ParameterDTO> Parameters { get; init; } = Array.Empty<ParameterDTO>();
}
