using View.Controllers.Abstract;
using View.DTOs;
using View.Services;

namespace View.Controllers.Entities;

[ApiController]
[Route("api/tour")]
public class TourController(ITourRepository repository, IRouteService routeService, ILogger<TourController> logger)
    : AController<Tour, CreateTourDto, ReadTourDto, UpdateTourDto>(repository, logger) {
    protected override bool IsOwner(Tour entity) =>
        TryGetCurrentUserId(out var userId) && entity.UserId == userId;

    [Authorize]
    [HttpPost]
    public override async Task<ActionResult<ReadTourDto>> CreateAsync(CreateTourDto entity, CancellationToken ct) {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Invalid User");

        Tour toCreate = entity.Adapt<Tour>();
        toCreate.UserId = userId;

        logger.LogDebug(
            "Fetching route for new Tour (user {UserId}, transport {Transport}, from [{FromLng},{FromLat}] to [{ToLng},{ToLat}])",
            userId, toCreate.TransportType, toCreate.FromLongitude, toCreate.FromLatitude, toCreate.ToLongitude,
            toCreate.ToLatitude);

        var route = await routeService.GetRouteAsync(
            toCreate.TransportType,
            toCreate.FromLongitude, toCreate.FromLatitude,
            toCreate.ToLongitude, toCreate.ToLatitude,
            ct);

        toCreate.Distance = route.DistanceKm;
        toCreate.Duration = route.DurationMinutes;
        toCreate.Coordinates = route.EncodedGeometry;

        var created = await repository.CreateAsync(toCreate, ct);
        logger.LogInformation("Created Tour {TourId} for user {UserId} ({DistanceKm} km, {DurationMin} min)",
            created.Id, userId, route.DistanceKm, route.DurationMinutes);

        return Ok(created.Adapt<ReadTourDto>());
    }

    [Authorize]
    [HttpPut("{id}")]
    public override async Task<ActionResult<ReadTourDto>> UpdateAsync(Guid id, UpdateTourDto record,
        CancellationToken ct) {
        Tour? data = await repository.ReadAsync(id, ct);

        if (data is null) {
            logger.LogWarning("Tour not found for update: {TourId}", id);
            return NotFound();
        }

        if (!IsOwner(data)) {
            logger.LogWarning("Tour {TourId} update denied — not owned by current user.", id);
            return NotFound();
        }

        // ChildFriendliness is a server-side field the client does not manage;
        // preserve the stored value so Adapt doesn't reset it to the DTO default.
        var childFriendliness = data.ChildFriendliness;
        record.Adapt(data);
        data.ChildFriendliness = childFriendliness;

        logger.LogDebug(
            "Fetching route for Tour {TourId} update (transport {Transport}, from [{FromLng},{FromLat}] to [{ToLng},{ToLat}])",
            id, data.TransportType, data.FromLongitude, data.FromLatitude, data.ToLongitude, data.ToLatitude);

        var route = await routeService.GetRouteAsync(
            data.TransportType,
            data.FromLongitude, data.FromLatitude,
            data.ToLongitude, data.ToLatitude,
            ct);

        data.Distance = route.DistanceKm;
        data.Duration = route.DurationMinutes;
        data.Coordinates = route.EncodedGeometry;
        data.UpdatedAtUtc = DateTime.UtcNow;

        await repository.UpdateAsync(data, ct);
        logger.LogInformation("Updated Tour {TourId} ({DistanceKm} km, {DurationMin} min)", id, route.DistanceKm,
            route.DurationMinutes);

        // Re-read with logs so the response matches the "mine" endpoint (UpdateAsync clears the change tracker).
        var updated = await repository.ReadWithLogsAsync(id, ct);
        return Ok(updated!.Adapt<ReadTourDto>());
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<ReadTourDto>>> ReadOwnAsync(CancellationToken ct) {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Invalid User");

        List<Tour> results = await repository.ReadByUserWithLogsAsync(userId, ct);
        logger.LogInformation("Returned {Count} tours for user {UserId}", results.Count, userId);

        return Ok(results.Adapt<List<ReadTourDto>>());
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<List<ReadTourDto>>> SearchAsync([FromQuery] string q, CancellationToken ct) {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query must not be empty.");

        List<Tour> results = await repository.SearchAsync(q, ct);
        var owned = results.Where(IsOwner).ToList();
        logger.LogInformation("Tour search '{Query}' returned {Count} results", q, owned.Count);

        return Ok(owned.Adapt<List<ReadTourDto>>());
    }
}