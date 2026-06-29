namespace Domain.Repositories.Implementations;

public class TourRepository(TourPlannerDbContext context) : ARepository<Tour>(context), ITourRepository {
	public async Task<List<Tour>> SearchAsync(string query, CancellationToken ct) {
		string prefixQuery = BuildPrefixTsQuery(query);

		if (string.IsNullOrEmpty(prefixQuery))
			return [];

		return await Table
			.Where(t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", prefixQuery)))
			.OrderByDescending(t => t.SearchVector.Rank(EF.Functions.ToTsQuery("english", prefixQuery)))
			.ToListAsync(ct);
	}
}