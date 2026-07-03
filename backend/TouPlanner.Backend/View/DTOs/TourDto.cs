using Model;

namespace View.DTOs;

public record CreateTourDto(
	[Required][MaxLength(150)] string Name,
	[MaxLength(2000)] string? Description,
	[Range(-180.0, 180.0)] double FromLongitude,
	[Range(-90.0, 90.0)] double FromLatitude,
	[Range(-180.0, 180.0)] double ToLongitude,
	[Range(-90.0, 90.0)] double ToLatitude,
	[EnumDataType(typeof(TransportType))] TransportType TransportType,
	double Distance,
	int Duration,
	string? Coordinates,
	[Range(0.0, 5.0)] double ChildFriendliness
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
	[Required][MaxLength(150)] string Name,
	[MaxLength(2000)] string? Description,
	[Range(-180.0, 180.0)] double FromLongitude,
	[Range(-90.0, 90.0)] double FromLatitude,
	[Range(-180.0, 180.0)] double ToLongitude,
	[Range(-90.0, 90.0)] double ToLatitude,
	[EnumDataType(typeof(TransportType))] TransportType TransportType,
	double Distance,
	int Duration,
	string? Coordinates,
	[Range(0.0, 5.0)] double ChildFriendliness
);
