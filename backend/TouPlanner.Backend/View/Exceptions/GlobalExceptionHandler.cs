using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace View.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler {
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct) {
		var (statusCode, title) = exception switch {
			OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Request Timeout"),
			_ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
		};

		logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

		var problem = new ProblemDetails {
			Status = statusCode,
			Title = title,
			Detail = exception.Message
		};

		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(problem, ct);

		return true;
	}
}
