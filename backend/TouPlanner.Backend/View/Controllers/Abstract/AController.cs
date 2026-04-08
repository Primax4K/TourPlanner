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
	[Authorize]
	[HttpGet("{id}")]
	public virtual async Task<ActionResult<TReadEntityDto>> ReadAsync(Guid id, CancellationToken ct) {
		try {
			TEntity? data = await repository.ReadAsync(id, ct);

			if (data is null) {
				logger.LogInformation($"Invalid Request: Entity not present - {id}");
				return NotFound();
			}

			logger.LogInformation($"Sending Entity: {id}");
			return Ok(data.Adapt<TReadEntityDto>());
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
	[HttpPut("{id}")]
	public virtual async Task<ActionResult> UpdateAsync(Guid id, TUpdateEntityDto record, CancellationToken ct) {
		try {
			TEntity? data = await repository.ReadAsync(id, ct);

			if (data is null) {
				logger.LogInformation($"Invalid Request: Entity not present - {id}");
				return NotFound();
			}

			await repository.UpdateAsync(record.Adapt<TEntity>(), ct);
			logger.LogInformation($"Updated Entity: {id}");

			return NoContent();
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
	[HttpDelete("{id}")]
	public virtual async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) {
		try {
			TEntity? data = await repository.ReadAsync(id, ct);

			if (data is null) {
				return NotFound();
			}

			await repository.DeleteAsync(data, ct);
			logger.LogInformation($"Deleted Entity: {id}");

			return NoContent();
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
	[HttpPost]
	public virtual async Task<ActionResult<TReadEntityDto>> CreateAsync(TCreateEntityDto entity, CancellationToken ct) {
		try {
			return Ok((await repository.CreateAsync(entity.Adapt<TEntity>(), ct)).Adapt<TReadEntityDto>());
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