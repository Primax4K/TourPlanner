namespace View.Responses;

public record MeResponse(Guid Id, string Username, string Email, DateTime CreatedAtUtc);
