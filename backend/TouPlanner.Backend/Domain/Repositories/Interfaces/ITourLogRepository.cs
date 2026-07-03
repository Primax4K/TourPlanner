namespace Domain.Repositories.Interfaces;

public interface ITourLogRepository : IRepository<TourLog> {
	Task<List<TourLog>> SearchAsync(string query, CancellationToken ct);
}