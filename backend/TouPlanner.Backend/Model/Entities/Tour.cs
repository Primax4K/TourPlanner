namespace Model.Entities;

[Table("Tours")]
public class Tour {
	public Guid Id { get; set; } = Guid.NewGuid();

	[Required] [MaxLength(150)] public string Name { get; set; } = string.Empty;

	[MaxLength(2000)] public string? Description { get; set; }

	[Required] [MaxLength(200)] public string From { get; set; } = string.Empty;

	[Required] [MaxLength(200)] public string To { get; set; } = string.Empty;

	[Required] public TransportType TransportType { get; set; }

	public double DistanceKm { get; set; }

	public int EstimatedTimeMinutes { get; set; }

	[MaxLength(4000)] public string? RouteInformation { get; set; }

	[MaxLength(500)] public string? MapImagePath { get; set; }

	// Computed/derived fields persisted for searching/filtering
	public int Popularity { get; set; }

	public double ChildFriendliness { get; set; }

	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAtUtc { get; set; }

	[Required] public Guid UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;

	public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();
}