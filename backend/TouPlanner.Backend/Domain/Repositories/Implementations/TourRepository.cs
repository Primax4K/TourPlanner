namespace Domain.Repositories.Implementations;

public class TourRepository(TourPlannerDbContext context) : ARepository<Tour>(context), ITourRepository {
	public async Task<List<Tour>> ReadByUserWithLogsAsync(Guid userId, CancellationToken ct) {
		return await Table
			.Include(t => t.TourLogs)
			.Where(t => t.UserId == userId)
			.ToListAsync(ct);
	}

	public async Task<Tour?> ReadWithLogsAsync(Guid id, CancellationToken ct) {
		return await Table
			.Include(t => t.TourLogs)
			.FirstOrDefaultAsync(t => t.Id == id, ct);
	}

	public async Task<List<Tour>> SearchAsync(string query, CancellationToken ct) {
		string prefixQuery = BuildPrefixTsQuery(query);

		if (string.IsNullOrEmpty(prefixQuery))
			return [];

		return await Table
			.Include(t => t.TourLogs)
			.Where(t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", prefixQuery)))
			.OrderByDescending(t => t.SearchVector.Rank(EF.Functions.ToTsQuery("english", prefixQuery)))
			.ToListAsync(ct);
	}
}