using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CalculatorRepository : ICalculatorRepository
{
    private readonly AppDbContext _db;
    public CalculatorRepository(AppDbContext db) => _db = db;

    public Task AddAsync(CalculatorCalculation calc, CancellationToken ct = default)
    {
        _db.Calculations.Add( calc );
        return _db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<CalculatorCalculation>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _db.Calculations
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
        return list;
    }
    public async Task<CalculatorCalculation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Calculations
                               .AsNoTracking()
                               .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
    public async Task<CalculatorCalculation> UpdateAsync(CalculatorCalculation entity, CancellationToken ct = default)
    {
        _db.Calculations.Update(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var toDelete = await _db.Calculations.FindAsync(new object[] { id }, ct);
        if (toDelete != null)
        {
            _db.Calculations.Remove(toDelete);
            await _db.SaveChangesAsync(ct);
        }
    }
}
