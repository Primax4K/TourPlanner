namespace View.DTOs;

public record CreateTourLogDto(
	Guid TourId,
	DateTime DateTimeUtc,
	string? Comment,
	int Difficulty,
	double TotalDistanceKm,
	int TotalTimeMinutes,
	int Rating
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
	DateTime DateTimeUtc,
	string? Comment,
	int Difficulty,
	double TotalDistanceKm,
	int TotalTimeMinutes,
	int Rating
);
