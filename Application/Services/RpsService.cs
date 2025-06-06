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


        var last = await _repo.GetLastestAsync(ct);


        int prevGames = last?.Games ?? 0;
        int prevWins = last?.Wins ?? 0;
        int prevLosses = last?.Losses ?? 0;
        int prevTies = last?.Ties ?? 0;


        int newGames = prevGames + 1;
        int newWins = prevWins + (outcome == GameOutcome.Win ? 1 : 0);
        int newLosses = prevLosses + (outcome == GameOutcome.Lose ? 1 : 0);
        int newTies = prevTies + (outcome == GameOutcome.Draw ? 1 : 0);


        var game = new RPSGame
        {
            PlayerMove = cmd.PlayerMove,
            ComputerMove = cpuMove,
            Outcome = outcome,
            Games = newGames,
            Wins = newWins,
            Losses = newLosses,
            Ties = newTies
        };

        await _repo.AddAsync(game, ct);

        return _mapper.Map<RpsGameDTO>(game);
    }
    public async Task<IReadOnlyList<RpsGameDTO>> GetHistoryAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<RpsGameDTO>>(list);
    }

    public async Task<double> GetWinRateAsync(CancellationToken ct = default)
    {
        var last = await _repo.GetLastestAsync(ct);
        if (last == null || last.Games == 0)
            return 0.0;
        return last.Wins / (double)last.Games;
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
