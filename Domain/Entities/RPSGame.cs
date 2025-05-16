using Domain.Commons;
using Shared.Enums;

namespace Domain.Entities
{
    public class RPSGame : BaseEntity
    {
        public RPSMove PlayerMove { get; set; }
        public RPSMove ComputerMove { get; set; }
        public GameOutcome Outcome { get; set; }
    }
}
