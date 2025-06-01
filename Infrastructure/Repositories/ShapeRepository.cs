using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShapeRepository : IShapeRepository
{
    private readonly AppDbContext _db; 

    public ShapeRepository(AppDbContext db) => _db = db;

    public async Task<ShapeCalculation> AddAsync(ShapeCalculation shape, CancellationToken ct = default)
    {
        _db.ShapeCalculations.Add(shape);
        await _db.SaveChangesAsync(ct);

        return shape;
    }

    public async Task<IReadOnlyList<ShapeCalculation>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _db.ShapeCalculations
            .Include(s => s.ShapeParameters)
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
        return list;
    }

    public async Task<ShapeCalculation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var shape = await _db.ShapeCalculations
            .Include (s => s.ShapeParameters)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        return shape;
    }

    public async Task UpdateAsync(ShapeCalculation shape, CancellationToken ct = default)
    {
        _db.ShapeCalculations.Update(shape);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var toDelete = await _db.ShapeCalculations.FindAsync(new object[] { id }, ct);
        if (toDelete != null)
        {
            _db.ShapeCalculations.Remove(toDelete);
            await _db.SaveChangesAsync(ct);
        }
    }
}
