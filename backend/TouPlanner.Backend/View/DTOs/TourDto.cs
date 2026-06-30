using Model;

namespace View.DTOs;

public record CreateTourDto(
	string Name,
	string? Description,
	double FromLongitude,
	double FromLatitude,
	double ToLongitude,
	double ToLatitude,
	TransportType TransportType,
	double Distance,
	int Duration,
	string? Coordinates,
	double ChildFriendliness
);

public record ReadTourDto(
	Guid Id,
	string Name,
	string? Description,
	double FromLongitude,
	double FromLatitude,
	double ToLongitude,
	double ToLatitude,
	TransportType TransportType,
	double Distance,
	int Duration,
	string? Coordinates,
	int Popularity,
	double ChildFriendliness,
	DateTime CreatedAtUtc,
	DateTime? UpdatedAtUtc,
	Guid UserId,
	List<ReadTourLogDto> TourLogs
);

public record UpdateTourDto(
	string Name,
	string? Description,
	double FromLongitude,
	double FromLatitude,
	double ToLongitude,
	double ToLatitude,
	TransportType TransportType,
	double Distance,
	int Duration,
	string? Coordinates,
	double ChildFriendliness
);
