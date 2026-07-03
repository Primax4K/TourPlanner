using System.Net.Http.Json;
using System.Text.Json;
using Model;
using View.Exceptions;

namespace View.Services;

public class RouteService(HttpClient httpClient, IConfiguration config) : IRouteService {
	private static string ProfileFor(TransportType type) => type switch {
		TransportType.Car     => "driving-car",
		TransportType.Cycling => "cycling-regular",
		TransportType.Walking => "foot-walking",
		_                     => "driving-car"
	};

	public async Task<RouteData> GetRouteAsync(
		TransportType transportType,
		double fromLongitude, double fromLatitude,
		double toLongitude, double toLatitude,
		CancellationToken ct) {

		var profile = ProfileFor(transportType);
		var apiKey = config["OpenRouteService:ApiKey"]
			?? throw new InvalidOperationException("OpenRouteService:ApiKey is not configured.");

		var body = new {
			coordinates = new[] {
				new[] { fromLongitude, fromLatitude },
				new[] { toLongitude, toLatitude }
			}
		};

		var request = new HttpRequestMessage(HttpMethod.Post,
			$"https://api.openrouteservice.org/v2/directions/{profile}") {
			Content = JsonContent.Create(body)
		};
		request.Headers.TryAddWithoutValidation("Authorization", apiKey);

		try {
			var response = await httpClient.SendAsync(request, ct);
			response.EnsureSuccessStatusCode();

			using var doc = await JsonDocument.ParseAsync(
				await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

			var route   = doc.RootElement.GetProperty("routes")[0];
			var summary = route.GetProperty("summary");

			return new RouteData(
				DistanceKm:       Math.Round(summary.GetProperty("distance").GetDouble() / 1000.0, 2),
				DurationMinutes:  (int)Math.Round(summary.GetProperty("duration").GetDouble() / 60.0),
				EncodedGeometry:  route.GetProperty("geometry").GetString()!
			);
		}
		catch (HttpRequestException ex) {
			throw new RouteServiceException("Failed to retrieve route from OpenRouteService.", ex);
		}
	}
}
