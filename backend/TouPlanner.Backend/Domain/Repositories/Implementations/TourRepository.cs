namespace Domain.Repositories.Implementations;

public class TourRepository(TourPlannerDbContext context) : ARepository<Tour>(context), ITourRepository { }