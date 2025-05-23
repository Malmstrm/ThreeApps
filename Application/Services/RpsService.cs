using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Shared.Enums;

namespace Application.Services;

public class RpsService : IRpsService
{
    private readonly IRpsRepository _repo;
    private readonly IMapper _mapper;
    private readonly Random _rng = new();
    public RpsService(IRpsRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    public async Task<RpsGameDTO> PlayAsync(PlayRpsCommand cmd, CancellationToken ct = default)
    {
        var cpuMove = (RPSMove)_rng.Next(0, 3);
        var outcome = Evaluate(cmd.PlayerMove, cpuMove);

        var game = new RPSGame()
        {
            PlayerMove = cmd.PlayerMove,
            ComputerMove = cpuMove,
            Outcome = outcome
        };

        await _repo.AddAsync(game);

        return _mapper.Map<RpsGameDTO>(game);
    }
    public async Task<IReadOnlyList<RpsGameDTO>> GetHistoryAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<RpsGameDTO>>(list);
    }

    public async Task<double> GetWinRateAsync(CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(_ => true, ct);
        if(total == 0) return 0;
        var wins = await _repo.CountAsync(g => g.Outcome == GameOutcome.Win, ct);
        return wins / (double)total;
    }
    private static GameOutcome Evaluate(RPSMove p, RPSMove c) => (p, c) switch
    {
        _ when p == c => GameOutcome.Draw,
        (RPSMove.Rock, RPSMove.Scissors) or
        (RPSMove.Paper, RPSMove.Rock) or
        (RPSMove.Scissors, RPSMove.Paper) => GameOutcome.Win,
        _ => GameOutcome.Lose
    };

}
