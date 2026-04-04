namespace Domain.Repositories.Implementations;

public class TourLogRepository(TourPlannerDbContext context) : ARepository<TourLog>(context), ITourLogRepository { }