using Shared.Enums;


namespace Application.DTOs;

public record RpsGameDTO
{
    public int Id { get; init; }
    public RPSMove PlayerMove { get; init; }
    public RPSMove ComputerMove { get; init; }
    public GameOutcome Outcome { get; init; }
    public DateOnly PlayedAt { get; init; }
}
