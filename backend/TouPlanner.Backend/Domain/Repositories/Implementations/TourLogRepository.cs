namespace Domain.Repositories.Implementations;

public class TourLogRepository(TourPlannerDbContext context) : ARepository<TourLog>(context), ITourLogRepository {
	public async Task<List<TourLog>> SearchAsync(string query, CancellationToken ct) {
		string prefixQuery = BuildPrefixTsQuery(query);

		if (string.IsNullOrEmpty(prefixQuery))
			return [];

		return await Table
			.Where(t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", prefixQuery)))
			.OrderByDescending(t => t.SearchVector.Rank(EF.Functions.ToTsQuery("english", prefixQuery)))
			.ToListAsync(ct);
	}
}