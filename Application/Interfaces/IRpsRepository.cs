using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRpsRepository
    {
        Task AddAsync(RPSGame game, CancellationToken ct = default);
        Task<IReadOnlyList<RPSGame>> GetAllAsync(CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<RPSGame, bool>> predicate, CancellationToken ct = default);

        Task<RPSGame?> GetLatestAsync(CancellationToken ct = default);
    }
}
