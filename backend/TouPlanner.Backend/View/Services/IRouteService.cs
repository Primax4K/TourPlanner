using Model;

namespace View.Services;

public record RouteData(double DistanceKm, int DurationMinutes, string EncodedGeometry);

public interface IRouteService {
	Task<RouteData> GetRouteAsync(
		TransportType transportType,
		double fromLongitude, double fromLatitude,
		double toLongitude, double toLatitude,
		CancellationToken ct);
}
