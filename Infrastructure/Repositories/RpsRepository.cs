using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class RpsRepository : IRpsRepository
    {
        private readonly AppDbContext _dbContext;
        public RpsRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public Task AddAsync(RPSGame game, CancellationToken ct = default)
        {
            _dbContext.RPSGames.Add(game);
            return _dbContext.SaveChangesAsync(ct);
        }

        public Task<int> CountAsync(Expression<Func<RPSGame, bool>> predicate, CancellationToken ct = default) =>
            _dbContext.RPSGames.CountAsync(predicate, ct);

        public async Task<IReadOnlyList<RPSGame>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _dbContext.RPSGames
                .AsNoTracking()
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync(ct);

            return list;
        }
        public async Task<RPSGame?> GetLatestAsync(CancellationToken ct = default)
        {
            return await _dbContext.RPSGames
                .AsNoTracking()
                .OrderByDescending(g => g.Id)
                .FirstOrDefaultAsync(ct);
        }
    }
}