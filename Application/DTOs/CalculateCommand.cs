using Shared.Enums;
namespace Application.DTOs;

public record CalculateCommand(
    double Operand1,
    double? Operand2,
    CalculatorOperator Operator);