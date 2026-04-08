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
	double DistanceKm,
	int EstimatedTimeMinutes,
	string? RouteInformation,
	string? MapImagePath,
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
	double DistanceKm,
	int EstimatedTimeMinutes,
	string? RouteInformation,
	string? MapImagePath,
	int Popularity,
	double ChildFriendliness,
	DateTime CreatedAtUtc,
	DateTime? UpdatedAtUtc,
	Guid UserId
);

public record UpdateTourDto(
	string Name,
	string? Description,
	double FromLongitude,
	double FromLatitude,
	double ToLongitude,
	double ToLatitude,
	TransportType TransportType,
	double DistanceKm,
	int EstimatedTimeMinutes,
	string? RouteInformation,
	string? MapImagePath,
	double ChildFriendliness
);