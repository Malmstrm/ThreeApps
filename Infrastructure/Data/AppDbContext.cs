using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<ShapeCalculation> ShapeCalculations { get; set; } = null!;
    public DbSet<ShapeParameter> ShapeParameters { get; set; } = null!;
    public DbSet<CalculatorCalculation> Calculations { get; set; } = null!;
    public DbSet<RPSGame> RPSGames { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enums as string for readability
        modelBuilder.Entity<ShapeCalculation>().Property(s => s.ShapeType).HasConversion<string>();
        modelBuilder.Entity<ShapeParameter>().Property(p => p.ParameterType).HasConversion<string>();
        modelBuilder.Entity<CalculatorCalculation>().Property(c => c.Operator).HasConversion<string>();
        modelBuilder.Entity<RPSGame>().Property(g => g.PlayerMove).HasConversion<string>();
        modelBuilder.Entity<RPSGame>().Property(g => g.ComputerMove).HasConversion<string>();
        modelBuilder.Entity<RPSGame>().Property(g => g.Outcome).HasConversion<string>();

        modelBuilder.Entity<ShapeParameter>()
            .HasOne(p => p.ShapeCalculation)
            .WithMany(s => s.ShapeParameters)
            .HasForeignKey(p => p.ShapeCalculationId);
    }
}
