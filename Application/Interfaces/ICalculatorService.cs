using Application.DTOs;

namespace Application.Interfaces;

public interface ICalculatorService
{
    Task<CalculationDTO> CalculationAsync(CalculateCommand cmd, CancellationToken ct = default);
    Task<IReadOnlyList<CalculationDTO>> GetHistoryAsync(CancellationToken ct = default);
}
