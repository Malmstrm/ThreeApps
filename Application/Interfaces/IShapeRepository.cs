using Domain.Entities;

namespace Application.Interfaces;

public interface IShapeRepository
{
    Task<ShapeCalculation> AddAsync(ShapeCalculation shape, CancellationToken ct = default);

    Task<IReadOnlyList<ShapeCalculation>> GetAllAsync(CancellationToken ct = default);

    Task<ShapeCalculation?> GetByIdAsync(int id, CancellationToken ct = default);

    Task UpdateAsync(ShapeCalculation shape, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);
}
