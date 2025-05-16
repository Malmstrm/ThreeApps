namespace Domain.Commons
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
