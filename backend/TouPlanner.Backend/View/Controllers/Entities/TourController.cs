using System.Security.Claims;
using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using View.Controllers.Abstract;
using View.DTOs;

namespace View.Controllers.Entities;

[ApiController]
[Route("api/tour")]
public class TourController(ITourRepository repository, ILogger<TourController> logger)
	: AController<Tour, CreateTourDto, ReadTourDto, UpdateTourDto>(repository, logger) {

	protected override bool IsOwner(Tour entity) =>
		TryGetCurrentUserId(out var userId) && entity.UserId == userId;

	[Authorize]
	[HttpPost]
	public override async Task<ActionResult<ReadTourDto>> CreateAsync(CreateTourDto entity, CancellationToken ct) {
		try {
			Tour toCreate = entity.Adapt<Tour>();

			if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
				return Unauthorized("Invalid User");
			
			toCreate.UserId = userId;

			return Ok((await repository.CreateAsync(toCreate, ct)).Adapt<ReadTourDto>());
		}
		catch (OperationCanceledException) {
			logger.LogError("Zeitüberschreitung der Anforderungen!");
			return StatusCode(408);
		}
		catch (Exception e) {
			logger.LogError(e, "Fehler beim Abrufen der Entität!");
			return Problem("Fehler beim Abrufen der Entität!");
		}
	}

	[Authorize]
	[HttpGet("mine")]
	public async Task<ActionResult<List<ReadTourDto>>> ReadOwnAsync(CancellationToken ct) {
		try {
			if (!TryGetCurrentUserId(out var userId))
				return Unauthorized("Invalid User");

			List<Tour> results = await repository.ReadAsync(t => t.UserId == userId, ct);

			return Ok(results.Adapt<List<ReadTourDto>>());
		}
		catch (OperationCanceledException) {
			logger.LogError("Zeitüberschreitung der Anforderungen!");
			return StatusCode(408);
		}
		catch (Exception e) {
			logger.LogError(e, "Fehler beim Abrufen der Entität!");
			return Problem("Fehler beim Abrufen der Entität!");
		}
	}

	[Authorize]
	[HttpGet("search")]
	public async Task<ActionResult<List<ReadTourDto>>> SearchAsync([FromQuery] string q, CancellationToken ct) {
		try {
			if (string.IsNullOrWhiteSpace(q))
				return BadRequest("Query must not be empty.");

			List<Tour> results = await repository.SearchAsync(q, ct);

			return Ok(results.Where(IsOwner).Adapt<List<ReadTourDto>>());
		}
		catch (OperationCanceledException) {
			logger.LogError("Zeitüberschreitung der Anforderungen!");
			return StatusCode(408);
		}
		catch (Exception e) {
			logger.LogError(e, "Fehler beim Abrufen der Entität!");
			return Problem("Fehler beim Abrufen der Entität!");
		}
	}
}