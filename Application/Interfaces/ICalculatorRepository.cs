using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICalculatorRepository
    {
        Task AddAsync(CalculatorCalculation calc, CancellationToken ct = default);
        Task<IReadOnlyList<CalculatorCalculation>> GetAllAsync(CancellationToken ct = default);
    }
}
