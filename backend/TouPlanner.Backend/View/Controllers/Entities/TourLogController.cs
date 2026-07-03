using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using View.Controllers.Abstract;
using View.DTOs;

namespace View.Controllers.Entities;

[ApiController]
[Route("api/tourlog")]
public class TourLogController(ITourLogRepository repository, ITourRepository tourRepository, ILogger<TourLogController> logger)
	: AController<TourLog, CreateTourLogDto, ReadTourLogDto, UpdateTourLogDto>(repository, logger) {

	[Authorize]
	[HttpPost]
	public override async Task<ActionResult<ReadTourLogDto>> CreateAsync(CreateTourLogDto entity, CancellationToken ct) {
		if (!await tourRepository.ExistsAsync(entity.TourId, ct))
			return NotFound($"Tour {entity.TourId} does not exist.");

		if (!TryGetCurrentUserId(out var userId))
			return Unauthorized("Invalid User");

		TourLog toCreate = entity.Adapt<TourLog>();
		toCreate.UserId = userId;

		var created = await repository.CreateAsync(toCreate, ct);
		await UpdatePopularityAsync(entity.TourId, ct);

		return Ok(created.Adapt<ReadTourLogDto>());
	}

	[Authorize]
	[HttpDelete("{id}")]
	public override async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) {
		TourLog? log = await repository.ReadAsync(id, ct);

		if (log is null)
			return NotFound();

		var tourId = log.TourId;

		await repository.DeleteAsync(log, ct);
		await UpdatePopularityAsync(tourId, ct);

		return NoContent();
	}

	[Authorize]
	[HttpGet("search")]
	public async Task<ActionResult<List<ReadTourLogDto>>> SearchAsync([FromQuery] string q, CancellationToken ct) {
		if (string.IsNullOrWhiteSpace(q))
			return BadRequest("Query must not be empty.");

		List<TourLog> results = await repository.SearchAsync(q, ct);

		return Ok(results.Adapt<List<ReadTourLogDto>>());
	}

	private async Task UpdatePopularityAsync(Guid tourId, CancellationToken ct) {
		Tour? tour = await tourRepository.ReadAsync(tourId, ct);
		if (tour is null) return;

		tour.Popularity = (await repository.ReadAsync(t => t.TourId == tourId, ct)).Count;
		await tourRepository.UpdateAsync(tour, ct);
	}
}
