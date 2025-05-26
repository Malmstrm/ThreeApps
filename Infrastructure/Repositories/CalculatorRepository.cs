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


}
