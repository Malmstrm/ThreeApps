using Application.DTOs;

namespace Application.Interfaces;

public interface IShapeService
{
    Task<ShapeCalculationDTO> CreateAsync(CreateShapeCommand cmd, CancellationToken ct = default);
    Task<IReadOnlyList<ShapeCalculationDTO>> GetAllAsync(CancellationToken ct = default);
    Task<ShapeCalculationDTO?> GetByIdAsyc(int id, CancellationToken ct = default);
    Task UpdateAsync(UpdateShapeCommand cmd, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
