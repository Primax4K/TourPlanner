namespace View.Controllers.Abstract;

public abstract class AController<TEntity, TCreateEntityDto, TReadEntityDto, TUpdateEntityDto>(
    IRepository<TEntity> repository,
    ILogger<AController<TEntity, TCreateEntityDto, TReadEntityDto, TUpdateEntityDto>> logger) : ControllerBase
    where TEntity : class
    where TCreateEntityDto : class
    where TUpdateEntityDto : class
    where TReadEntityDto : class {
    protected abstract bool IsOwner(TEntity entity);

    protected bool TryGetCurrentUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

    [Authorize]
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TReadEntityDto>> ReadAsync(Guid id, CancellationToken ct) {
        TEntity? data = await repository.ReadAsync(id, ct);

        if (data is null) {
            logger.LogWarning("{Entity} not found: {Id}", typeof(TEntity).Name, id);
            return NotFound();
        }

        if (!IsOwner(data)) {
            logger.LogWarning("{Entity} {Id} access denied — not owned by current user.", typeof(TEntity).Name, id);
            return NotFound();
        }

        logger.LogInformation("Read {Entity}: {Id}", typeof(TEntity).Name, id);
        return Ok(data.Adapt<TReadEntityDto>());
    }

    [Authorize]
    [HttpPut("{id}")]
    public virtual async Task<ActionResult<TReadEntityDto>> UpdateAsync(Guid id, TUpdateEntityDto record,
        CancellationToken ct) {
        TEntity? data = await repository.ReadAsync(id, ct);

        if (data is null) {
            logger.LogWarning("{Entity} not found for update: {Id}", typeof(TEntity).Name, id);
            return NotFound();
        }

        if (!IsOwner(data)) {
            logger.LogWarning("{Entity} {Id} update denied — not owned by current user.", typeof(TEntity).Name, id);
            return NotFound();
        }

        record.Adapt(data);
        await repository.UpdateAsync(data, ct);
        logger.LogInformation("Updated {Entity}: {Id}", typeof(TEntity).Name, id);

        return Ok(data.Adapt<TReadEntityDto>());
    }

    [Authorize]
    [HttpDelete("{id}")]
    public virtual async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) {
        TEntity? data = await repository.ReadAsync(id, ct);

        if (data is null) {
            logger.LogWarning("{Entity} not found for delete: {Id}", typeof(TEntity).Name, id);
            return NotFound();
        }

        if (!IsOwner(data)) {
            logger.LogWarning("{Entity} {Id} delete denied — not owned by current user.", typeof(TEntity).Name, id);
            return NotFound();
        }

        await repository.DeleteAsync(data, ct);
        logger.LogInformation("Deleted {Entity}: {Id}", typeof(TEntity).Name, id);

        return NoContent();
    }

    [Authorize]
    [HttpPost]
    public virtual async Task<ActionResult<TReadEntityDto>> CreateAsync(TCreateEntityDto entity, CancellationToken ct) {
        var created = await repository.CreateAsync(entity.Adapt<TEntity>(), ct);
        logger.LogInformation("Created {Entity}", typeof(TEntity).Name);
        return Ok(created.Adapt<TReadEntityDto>());
    }
}