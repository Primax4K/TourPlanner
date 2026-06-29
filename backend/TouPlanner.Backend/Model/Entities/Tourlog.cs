namespace Model.Entities;

[Table("Tourlogs")]
public class TourLog {
	public Guid Id { get; set; } = Guid.NewGuid();

	[Required] public Guid TourId { get; set; }
	public Tour Tour { get; set; } = null!;

	[Required] public Guid UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;

	public DateTime DateTimeUtc { get; set; }

	[MaxLength(2000)] public string? Comment { get; set; }

	[Range(1, 5)] public int Difficulty { get; set; }

	public double TotalDistanceKm { get; set; }

	public int TotalTimeMinutes { get; set; }

	[Range(1, 5)] public int Rating { get; set; }

	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

	// Generated tsvector column for full-text search (Comment)
	public NpgsqlTsVector SearchVector { get; set; } = null!;
}