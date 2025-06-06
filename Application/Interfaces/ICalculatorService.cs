using Application.DTOs;

namespace Application.Interfaces;

public interface ICalculatorService
{
    Task<CalculationDTO> CalculationAsync(CalculateCommand cmd, CancellationToken ct = default);
    Task<IReadOnlyList<CalculationDTO>> GetHistoryAsync(CancellationToken ct = default);
    Task<CalculationDTO?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CalculationDTO> UpdateAsync(UpdateCalculationCommand cmd, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
