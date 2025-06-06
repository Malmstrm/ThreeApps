using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICalculatorRepository
    {
        Task AddAsync(CalculatorCalculation calc, CancellationToken ct = default);
        Task<IReadOnlyList<CalculatorCalculation>> GetAllAsync(CancellationToken ct = default);
        Task<CalculatorCalculation?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CalculatorCalculation> UpdateAsync(CalculatorCalculation entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
