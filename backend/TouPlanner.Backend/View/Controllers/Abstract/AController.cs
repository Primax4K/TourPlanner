using System.Security.Claims;
using Domain.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers.Abstract;

public abstract class AController<TEntity, TCreateEntityDto, TReadEntityDto, TUpdateEntityDto>(
	IRepository<TEntity> repository,
	ILogger<AController<TEntity, TCreateEntityDto, TReadEntityDto, TUpdateEntityDto>> logger) : ControllerBase
	where TEntity : class
	where TCreateEntityDto : class
	where TUpdateEntityDto : class
	where TReadEntityDto : class {

	protected virtual bool IsOwner(TEntity entity) => true;

	protected bool TryGetCurrentUserId(out Guid userId) =>
		Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

	[Authorize]
	[HttpGet("{id}")]
	public virtual async Task<ActionResult<TReadEntityDto>> ReadAsync(Guid id, CancellationToken ct) {
		TEntity? data = await repository.ReadAsync(id, ct);

		if (data is null) {
			logger.LogInformation("Invalid Request: Entity not present - {Id}", id);
			return NotFound();
		}

		if (!IsOwner(data)) {
			logger.LogInformation("Invalid Request: Entity not owned by current user - {Id}", id);
			return NotFound();
		}

		logger.LogInformation("Sending Entity: {Id}", id);
		return Ok(data.Adapt<TReadEntityDto>());
	}

	[Authorize]
	[HttpPut("{id}")]
	public virtual async Task<ActionResult<TReadEntityDto>> UpdateAsync(Guid id, TUpdateEntityDto record, CancellationToken ct) {
		TEntity? data = await repository.ReadAsync(id, ct);

		if (data is null) {
			logger.LogInformation("Invalid Request: Entity not present - {Id}", id);
			return NotFound();
		}

		if (!IsOwner(data)) {
			logger.LogInformation("Invalid Request: Entity not owned by current user - {Id}", id);
			return NotFound();
		}

		record.Adapt(data);
		await repository.UpdateAsync(data, ct);
		logger.LogInformation("Updated Entity: {Id}", id);

		return Ok(data.Adapt<TReadEntityDto>());
	}

	[Authorize]
	[HttpDelete("{id}")]
	public virtual async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) {
		TEntity? data = await repository.ReadAsync(id, ct);

		if (data is null)
			return NotFound();

		if (!IsOwner(data)) {
			logger.LogInformation("Invalid Request: Entity not owned by current user - {Id}", id);
			return NotFound();
		}

		await repository.DeleteAsync(data, ct);
		logger.LogInformation("Deleted Entity: {Id}", id);

		return NoContent();
	}

	[Authorize]
	[HttpPost]
	public virtual async Task<ActionResult<TReadEntityDto>> CreateAsync(TCreateEntityDto entity, CancellationToken ct) {
		return Ok((await repository.CreateAsync(entity.Adapt<TEntity>(), ct)).Adapt<TReadEntityDto>());
	}
}
