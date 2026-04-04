namespace Domain.Repositories.Implementations;

public class ApplicationUserRepository(TourPlannerDbContext context)
	: ARepository<ApplicationUser>(context), IApplicationUserRepository { }