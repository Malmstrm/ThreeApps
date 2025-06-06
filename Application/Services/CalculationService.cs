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
    public async Task<CalculationDTO?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null
            ? null
            : _mapper.Map<CalculationDTO>(entity);
    }
    public async Task<CalculationDTO> UpdateAsync(UpdateCalculationCommand cmd, CancellationToken ct = default)
    {

        var existing = await _repo.GetByIdAsync(cmd.Id, ct);
        if (existing == null)
            throw new ArgumentException($"No record with Id {cmd.Id}");


        double result = cmd.Operator switch
        {
            CalculatorOperator.Addition => cmd.Operand1 + cmd.Operand2!.Value,
            CalculatorOperator.Subtraction => cmd.Operand1 - cmd.Operand2!.Value,
            CalculatorOperator.Multiplication => cmd.Operand1 * cmd.Operand2!.Value,
            CalculatorOperator.Division => cmd.Operand1 / cmd.Operand2!.Value,
            CalculatorOperator.Modulus => cmd.Operand1 % cmd.Operand2!.Value,
            CalculatorOperator.SquareRoot => Math.Sqrt(cmd.Operand1),
            _ => throw new InvalidOperationException("Unknown operator")
        };
        result = Math.Round(result, 2);


        existing.FirstValue = cmd.Operand1;
        existing.SecondValue = cmd.Operator == CalculatorOperator.SquareRoot ? null : cmd.Operand2;
        existing.Operator = cmd.Operator;
        existing.Result = result;


        var updated = await _repo.UpdateAsync(existing, ct);
        return _mapper.Map<CalculationDTO>(updated);


    }
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
    }
}
