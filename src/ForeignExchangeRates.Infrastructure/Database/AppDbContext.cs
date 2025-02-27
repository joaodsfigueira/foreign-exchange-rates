using ForeignExchangeRates.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForeignExchangeRates.Infrastructure.Database;

public class AppDbContext : DbContext
{

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ExchangeRate>()
			.HasIndex(c => new { c.SourceCurrencyCode, c.TargetCurrencyCode })
			.IsUnique();

		modelBuilder.Entity<ExchangeRate>()
					.Property(er => er.ExchangeRateValue)
					.HasPrecision(18, 4);

		modelBuilder.Entity<ExchangeRate>()
					.Property(er => er.BidPrice)
					.HasPrecision(18, 4);

		modelBuilder.Entity<ExchangeRate>()
					.Property(er => er.AskPrice)
					.HasPrecision(18, 4);
	}

    public DbSet<ExchangeRate> ExchangeRates { get; set; }
	public DbSet<User> Users { get; set; }
}