using Domain.Exceptions;

namespace Domain.Repositories.Implementations;

public abstract class ARepository<TEntity>(TourPlannerDbContext context) : IRepository<TEntity>
	where TEntity : class {
	protected readonly TourPlannerDbContext Context = context;
	protected readonly DbSet<TEntity> Table = context.Set<TEntity>();

	// Builds a prefix-matching tsquery string (e.g. "ab" -> "ab:*") so partial words
	// like "ab" match stored lexemes like "abc". plainto_tsquery/PlainToTsQuery only
	// matches whole (stemmed) lexemes, so it can't do this on its own.
	protected static string BuildPrefixTsQuery(string query) {
		IEnumerable<string> terms = query
			.Split([' ', '\t', '\n'], StringSplitOptions.RemoveEmptyEntries)
			.Select(term => new string(term.Where(char.IsLetterOrDigit).ToArray()))
			.Where(term => term.Length > 0);

		return string.Join(" & ", terms.Select(term => $"{term}:*"));
	}

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
		try {
			Table.Add(entity);
			await Context.SaveChangesAsync(ct);
			return entity;
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to create entity.", ex);
		}
	}

	public virtual async Task<List<TEntity>> CreateAsync(List<TEntity> entity, CancellationToken ct) {
		try {
			Table.AddRange(entity);
			await Context.SaveChangesAsync(ct);
			return entity;
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to create entities.", ex);
		}
	}

	public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct) {
		try {
			Context.ChangeTracker.Clear();
			Table.Update(entity);
			await Context.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to update entity.", ex);
		}
	}

	public virtual async Task UpdateAsync(IEnumerable<TEntity> entity, CancellationToken ct) {
		try {
			Context.ChangeTracker.Clear();
			Table.UpdateRange(entity);
			await Context.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to update entities.", ex);
		}
	}

	public virtual async Task DeleteAsync(TEntity entity, CancellationToken ct) {
		try {
			Context.ChangeTracker.Clear();
			Table.Remove(entity);
			await Context.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to delete entity.", ex);
		}
	}

	public virtual async Task DeleteAsync(IEnumerable<TEntity> entity, CancellationToken ct) {
		try {
			Context.ChangeTracker.Clear();
			Table.RemoveRange(entity);
			await Context.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to delete entities.", ex);
		}
	}

	public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) {
		try {
			Context.ChangeTracker.Clear();
			Table.RemoveRange(Table.Where(filter));
			await Context.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) {
			throw new RepositoryException("Failed to delete entities.", ex);
		}
	}
}