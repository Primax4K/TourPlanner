using View.Controllers.Abstract;
using View.DTOs;

namespace View.Controllers.Entities;

[ApiController]
[Route("api/tourlog")]
public class TourLogController(
    ITourLogRepository repository,
    ITourRepository tourRepository,
    ILogger<TourLogController> logger)
    : AController<TourLog, CreateTourLogDto, ReadTourLogDto, UpdateTourLogDto>(repository, logger) {

    protected override bool IsOwner(TourLog entity) =>
        TryGetCurrentUserId(out var userId) && entity.UserId == userId;

    [Authorize]
    [HttpPost]
    public override async Task<ActionResult<ReadTourLogDto>>
        CreateAsync(CreateTourLogDto entity, CancellationToken ct) {
        if (!await tourRepository.ExistsAsync(entity.TourId, ct)) {
            logger.LogWarning("TourLog create failed — Tour {TourId} does not exist.", entity.TourId);
            return NotFound($"Tour {entity.TourId} does not exist.");
        }

        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Invalid User");

        TourLog toCreate = entity.Adapt<TourLog>();
        toCreate.UserId = userId;

        var created = await repository.CreateAsync(toCreate, ct);
        await UpdatePopularityAsync(entity.TourId, ct);
        logger.LogInformation("Created TourLog {LogId} for Tour {TourId} by user {UserId}", created.Id, entity.TourId,
            userId);

        return Ok(created.Adapt<ReadTourLogDto>());
    }

    [Authorize]
    [HttpDelete("{id}")]
    public override async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) {
        TourLog? log = await repository.ReadAsync(id, ct);

        if (log is null) {
            logger.LogWarning("TourLog not found for delete: {LogId}", id);
            return NotFound();
        }

        if (!IsOwner(log)) {
            logger.LogWarning("TourLog {LogId} delete denied — not owned by current user.", id);
            return NotFound();
        }

        var tourId = log.TourId;

        await repository.DeleteAsync(log, ct);
        await UpdatePopularityAsync(tourId, ct);
        logger.LogInformation("Deleted TourLog {LogId} from Tour {TourId}", id, tourId);

        return NoContent();
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<List<ReadTourLogDto>>> SearchAsync([FromQuery] string q, CancellationToken ct) {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query must not be empty.");

        List<TourLog> results = await repository.SearchAsync(q, ct);
        logger.LogInformation("TourLog search '{Query}' returned {Count} results", q, results.Count);

        return Ok(results.Adapt<List<ReadTourLogDto>>());
    }

    private async Task UpdatePopularityAsync(Guid tourId, CancellationToken ct) {
        Tour? tour = await tourRepository.ReadAsync(tourId, ct);
        if (tour is null) return;

        tour.Popularity = (await repository.ReadAsync(t => t.TourId == tourId, ct)).Count;
        await tourRepository.UpdateAsync(tour, ct);
    }
}