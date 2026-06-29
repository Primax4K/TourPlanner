namespace Domain.Repositories.Interfaces;

public interface ITourRepository : IRepository<Tour> {
	Task<List<Tour>> SearchAsync(string query, CancellationToken ct);
}