namespace Domain.Repositories.Implementations;

public abstract class ARepository<TEntity>(TourPlannerDbContext context) : IRepository<TEntity>
	where TEntity : class {
	protected readonly TourPlannerDbContext Context = context;
	protected readonly DbSet<TEntity> Table = context.Set<TEntity>();

	public virtual async Task<List<TEntity>> ReadAsync(CancellationToken ct) {
		return await Table.ToListAsync(ct);
	}

	public virtual async Task<TEntity?> ReadAsync(Guid id, CancellationToken ct) {
		return await Table.FindAsync([id], ct);
	}

	public virtual async Task<List<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) {
		return await Table.Where(filter).ToListAsync(ct);
	}

	public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
		CancellationToken ct) {
		return await Table.FirstOrDefaultAsync(filter, ct);
	}

	public async Task<bool> ExistsAsync(Guid id, CancellationToken ct) {
		return await Table.FindAsync([id], ct) != null;
	}

	public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) {
		return await Table.IgnoreAutoIncludes().AnyAsync(filter, ct);
	}

	public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct) {
		Table.Add(entity);
		await Context.SaveChangesAsync(ct);
		return entity;
	}

	public virtual async Task<List<TEntity>> CreateAsync(List<TEntity> entity, CancellationToken ct) {
		Table.AddRange(entity);
		await Context.SaveChangesAsync(ct);
		return entity;
	}

	public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct) {
		Context.ChangeTracker.Clear();
		Table.Update(entity);
		await Context.SaveChangesAsync(ct);
	}

	public virtual async Task UpdateAsync(IEnumerable<TEntity> entity, CancellationToken ct) {
		Context.ChangeTracker.Clear();
		Table.UpdateRange(entity);
		await Context.SaveChangesAsync(ct);
	}

	public virtual async Task DeleteAsync(TEntity entity, CancellationToken ct) {
		Context.ChangeTracker.Clear();
		Table.Remove(entity);
		await Context.SaveChangesAsync(ct);
	}

	public virtual async Task DeleteAsync(IEnumerable<TEntity> entity, CancellationToken ct) {
		Context.ChangeTracker.Clear();
		Table.RemoveRange(entity);
		await Context.SaveChangesAsync(ct);
	}

	public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) {
		Context.ChangeTracker.Clear();
		Table.RemoveRange(Table.Where(filter));
		await Context.SaveChangesAsync(ct);
	}
}