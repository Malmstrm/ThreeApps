using Shared.Enums;

namespace Application.DTOs;

public class CalculationDTO
{
    public int Id { get; init; }
    public double Operand1 { get; init; }
    public double? Operand2 { get; init; }
    public CalculatorOperator Operator { get; init; }
    public double Result { get; init; }
    public DateOnly Date { get; init; }
}
