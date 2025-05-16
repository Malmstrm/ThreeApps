using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<ShapeCalculation> ShapeCalculations { get; set; } = null!;
    public DbSet<ShapeParameter> ShapeParameters { get; set; } = null!;
    public DbSet<CalculatorCalculation> Calculations { get; set; } = null!;
    public DbSet<RPSGame> RPSGames { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enums stored as string for readability

        // Shape Enums
        modelBuilder.Entity<ShapeCalculation>()
            .Property(s => s.ShapeType)
            .HasConversion<string>();

        modelBuilder.Entity<ShapeParameter>()
            .Property(p => p.ParameterType)
            .HasConversion<string>();

        // Calculator Enum
        modelBuilder.Entity<CalculatorCalculation>()
            .Property(c => c.Operator)
            .HasConversion<string>();

        // RPS Enums
        modelBuilder.Entity<RPSGame>()
            .Property(g => g.PlayerMove)
            .HasConversion<string>();

        modelBuilder.Entity<RPSGame>()
            .Property(g => g.ComputerMove)
            .HasConversion<string>();

        modelBuilder.Entity<RPSGame>()
            .Property(g => g.Outcome)
            .HasConversion<string>();

        // Relationships for Shapes
        modelBuilder.Entity<ShapeParameter>()
            .HasOne(p => p.ShapeCalculation)
            .WithMany(s => s.ShapeParameters)
            .HasForeignKey(p => p.ShapeCalculationId);
    }
}
