using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Shared.Enums;

namespace Application.Services;

public class CalculationService : ICalculatorService
{
    private readonly ICalculatorRepository _repo;
    private readonly IMapper _mapper;

    public CalculationService(ICalculatorRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    public async Task<CalculationDTO> CalculationAsync(CalculateCommand cmd, CancellationToken ct = default)
    {
        double result = cmd.Operator switch
        {
            CalculatorOperator.Addition => cmd.Operand1 + cmd.Operand2!.Value,
            CalculatorOperator.Subtraction => cmd.Operand1 - cmd.Operand2!.Value,
            CalculatorOperator.Multiplication => cmd.Operand1 * cmd.Operand2!.Value,
            CalculatorOperator.Division => cmd.Operand1 / cmd.Operand2!.Value,
            CalculatorOperator.Modulus => cmd.Operand1 % cmd.Operand2!.Value,
            CalculatorOperator.SquareRoot => Math.Sqrt(cmd.Operand1),
            _ => throw new InvalidOperationException("Oknown operator")
        };

        var entity = new CalculatorCalculation
        {
            FirstValue = cmd.Operand1,
            SecondValue = cmd.Operator == CalculatorOperator.SquareRoot ? null : cmd.Operand2,
            Operator = cmd.Operator,
            Result = Math.Round(result, 2)    // 2 decimaler
        };

        await _repo.AddAsync(entity, ct);
        return _mapper.Map<CalculationDTO>(entity);

    }
    public async Task<IReadOnlyList<CalculationDTO>> GetHistoryAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return list.Select(e => _mapper.Map<CalculationDTO>(e)).ToList();
    }
}
