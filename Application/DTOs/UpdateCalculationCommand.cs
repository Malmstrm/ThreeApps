using Shared.Enums;

namespace Application.DTOs;

public record UpdateCalculationCommand(
    int Id,
    double Operand1,
    double? Operand2,
    CalculatorOperator Operator);
