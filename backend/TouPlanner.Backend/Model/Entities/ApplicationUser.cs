namespace Model.Entities;

[Table("Users")]
public class ApplicationUser {
	public Guid Id { get; set; } = Guid.NewGuid();

	[Required] [MaxLength(100)] public string Username { get; set; } = string.Empty;

	[Required] [MaxLength(255)] public string Email { get; set; } = string.Empty;

	[Required] [MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;

	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

	public ICollection<Tour> Tours { get; set; } = new List<Tour>();
	public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();
}