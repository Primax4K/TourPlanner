using Model.Entities;

namespace Model.Configuration;

public class TourPlannerDbContext : DbContext {
	public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options)
		: base(options) { }

	public DbSet<ApplicationUser> Users { get; set; } = null!;
	public DbSet<Tour> Tours { get; set; } = null!;
	public DbSet<TourLog> TourLogs { get; set; } = null!;
	
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ApplicationUser>(entity => {
			entity.HasKey(x => x.Id);

			entity.HasIndex(x => x.Username).IsUnique();
			entity.HasIndex(x => x.Email).IsUnique();
		});

		modelBuilder.Entity<Tour>(entity => {
			entity.HasKey(x => x.Id);

			entity.Property(x => x.DistanceKm).HasPrecision(10, 2);
			entity.Property(x => x.ChildFriendliness).HasPrecision(5, 2);

			entity.HasOne(x => x.User)
				.WithMany(x => x.Tours)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasMany(x => x.TourLogs)
				.WithOne(x => x.Tour)
				.HasForeignKey(x => x.TourId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<TourLog>(entity => {
			entity.HasKey(x => x.Id);

			entity.Property(x => x.TotalDistanceKm).HasPrecision(10, 2);

			entity.HasOne(x => x.User)
				.WithMany(x => x.TourLogs)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasIndex(x => new { x.TourId, x.DateTimeUtc });
		});
	}
}