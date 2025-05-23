using Shared.Enums;


namespace Application.DTOs;

public record RpsGameDTO(
    int Id,
    RPSMove PlayerMove,
    RPSMove ComputerMove,
    GameOutcome Outcome,
    DateTime PlayedAt);
