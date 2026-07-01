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
[Route("api/tourlog")]
public class TourLogController(ITourLogRepository repository, ITourRepository tourRepository, ILogger<TourLogController> logger)
	: AController<TourLog, CreateTourLogDto, ReadTourLogDto, UpdateTourLogDto>(repository, logger) {

	[Authorize]
	[HttpPost]
	public override async Task<ActionResult<ReadTourLogDto>> CreateAsync(CreateTourLogDto entity, CancellationToken ct) {
		if (!await tourRepository.ExistsAsync(entity.TourId, ct))
			return NotFound($"Tour {entity.TourId} does not exist.");

		TourLog toCreate = entity.Adapt<TourLog>();

		if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
			return Unauthorized("Invalid User");

		toCreate.UserId = userId;

		return Ok((await repository.CreateAsync(toCreate, ct)).Adapt<ReadTourLogDto>());
	}

	[Authorize]
	[HttpGet("search")]
	public async Task<ActionResult<List<ReadTourLogDto>>> SearchAsync([FromQuery] string q, CancellationToken ct) {
		if (string.IsNullOrWhiteSpace(q))
			return BadRequest("Query must not be empty.");

		List<TourLog> results = await repository.SearchAsync(q, ct);

		return Ok(results.Adapt<List<ReadTourLogDto>>());
	}
}
