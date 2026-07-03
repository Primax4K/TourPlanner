namespace Domain.Repositories.Interfaces;

public interface ITourRepository : IRepository<Tour> {
	Task<List<Tour>> SearchAsync(string query, CancellationToken ct);
	Task<List<Tour>> ReadByUserWithLogsAsync(Guid userId, CancellationToken ct);
	Task<Tour?> ReadWithLogsAsync(Guid id, CancellationToken ct);
}