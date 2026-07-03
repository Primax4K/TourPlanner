namespace View.DTOs;

public record CreateTourLogDto(
	Guid TourId,
	DateTime DateTimeUtc,
	[MaxLength(2000)] string? Comment,
	[Range(1, 5)] int Difficulty,
	[Range(0.0, double.MaxValue)] double TotalDistanceKm,
	[Range(1, int.MaxValue)] int TotalTimeMinutes,
	[Range(1, 5)] int Rating
);

public record ReadTourLogDto(
	Guid Id,
	Guid TourId,
	Guid UserId,
	DateTime DateTimeUtc,
	string? Comment,
	int Difficulty,
	double TotalDistanceKm,
	int TotalTimeMinutes,
	int Rating,
	DateTime CreatedAtUtc
);

public record UpdateTourLogDto(
	[Required] DateTime DateTimeUtc,
	[MaxLength(2000)] string? Comment,
	[Range(1, 5)] int Difficulty,
	[Range(0.0, double.MaxValue)] double TotalDistanceKm,
	[Range(1, int.MaxValue)] int TotalTimeMinutes,
	[Range(1, 5)] int Rating
);
