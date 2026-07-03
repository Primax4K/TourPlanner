using Model.Configuration;

namespace View.Seeding;

public static class DataSeeder {
	public static async Task SeedAsync(IServiceProvider services) {
		await using var scope = services.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<TourPlannerDbContext>();

		// Idempotent: skip if test user already exists
		if (db.Users.Any(u => u.Username == "string"))
			return;

		var user = new ApplicationUser {
			Id = Guid.NewGuid(),
			Username = "string",
			Email = "string@string.com",
			PasswordHash = BCrypt.Net.BCrypt.HashPassword("string"),
			CreatedAtUtc = DateTime.UtcNow
		};

		var tourVienna = new Tour {
			Id = Guid.NewGuid(),
			Name = "Vienna City Walk",
			Description = "A relaxing walk through the historic center of Vienna.",
			FromLongitude = 16.3738,
			FromLatitude = 48.2082,
			ToLongitude = 16.3958,
			ToLatitude = 48.2201,
			TransportType = TransportType.Walking,
			Distance = 4.2,
			Duration = 60,
			Coordinates = null,
			Popularity = 3,
			ChildFriendliness = 4.5,
			UserId = user.Id,
			CreatedAtUtc = DateTime.UtcNow
		};

		var tourAlps = new Tour {
			Id = Guid.NewGuid(),
			Name = "Alpine Cycling Route",
			Description = "A challenging cycling route through the Austrian Alps.",
			FromLongitude = 13.0445,
			FromLatitude = 47.8095,
			ToLongitude = 13.1833,
			ToLatitude = 47.7167,
			TransportType = TransportType.Cycling,
			Distance = 38.7,
			Duration = 150,
			Coordinates = null,
			Popularity = 5,
			ChildFriendliness = 1.5,
			UserId = user.Id,
			CreatedAtUtc = DateTime.UtcNow
		};

		var tourLinz = new Tour {
			Id = Guid.NewGuid(),
			Name = "Linz to Salzburg Drive",
			Description = "Scenic motorway drive from Linz to Salzburg along the Danube valley.",
			FromLongitude = 14.2858,
			FromLatitude = 48.3069,
			ToLongitude = 13.0445,
			ToLatitude = 47.8095,
			TransportType = TransportType.Car,
			Distance = 127.0,
			Duration = 90,
			Coordinates = null,
			Popularity = 2,
			ChildFriendliness = 3.0,
			UserId = user.Id,
			CreatedAtUtc = DateTime.UtcNow
		};

		var logs = new List<TourLog> {
			// Vienna City Walk logs
			new() {
				Id = Guid.NewGuid(),
				TourId = tourVienna.Id,
				UserId = user.Id,
				DateTimeUtc = DateTime.UtcNow.AddDays(-10),
				Comment = "Beautiful morning walk, very enjoyable.",
				Difficulty = 1,
				TotalDistanceKm = 4.2,
				TotalTimeMinutes = 58,
				Rating = 5,
				CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
			},
			new() {
				Id = Guid.NewGuid(),
				TourId = tourVienna.Id,
				UserId = user.Id,
				DateTimeUtc = DateTime.UtcNow.AddDays(-3),
				Comment = "A bit crowded on the weekend but still great.",
				Difficulty = 1,
				TotalDistanceKm = 4.5,
				TotalTimeMinutes = 65,
				Rating = 4,
				CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
			},
			// Alpine Cycling logs
			new() {
				Id = Guid.NewGuid(),
				TourId = tourAlps.Id,
				UserId = user.Id,
				DateTimeUtc = DateTime.UtcNow.AddDays(-20),
				Comment = "Tough climb but the view from the top was worth every pedal.",
				Difficulty = 5,
				TotalDistanceKm = 39.1,
				TotalTimeMinutes = 162,
				Rating = 5,
				CreatedAtUtc = DateTime.UtcNow.AddDays(-20)
			},
			new() {
				Id = Guid.NewGuid(),
				TourId = tourAlps.Id,
				UserId = user.Id,
				DateTimeUtc = DateTime.UtcNow.AddDays(-7),
				Comment = "Rained halfway through, had to cut it short.",
				Difficulty = 4,
				TotalDistanceKm = 22.0,
				TotalTimeMinutes = 90,
				Rating = 3,
				CreatedAtUtc = DateTime.UtcNow.AddDays(-7)
			},
			// Linz to Salzburg log
			new() {
				Id = Guid.NewGuid(),
				TourId = tourLinz.Id,
				UserId = user.Id,
				DateTimeUtc = DateTime.UtcNow.AddDays(-5),
				Comment = "Smooth drive, traffic was light.",
				Difficulty = 1,
				TotalDistanceKm = 127.0,
				TotalTimeMinutes = 88,
				Rating = 4,
				CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
			}
		};

		db.Users.Add(user);
		db.Tours.AddRange(tourVienna, tourAlps, tourLinz);
		db.TourLogs.AddRange(logs);

		await db.SaveChangesAsync();
	}
}
