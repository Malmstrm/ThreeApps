using Application.DTOs;

namespace Application.Interfaces
{
    public interface IRpsService
    {
        Task<RpsGameDTO> PlayAsync(PlayRpsCommand cmd, CancellationToken ct = default);
        Task<IReadOnlyList<RpsGameDTO>> GetHistoryAsync(CancellationToken ct = default);
        Task<double> GetWinRateAsync(CancellationToken ct = default);
    }
}
