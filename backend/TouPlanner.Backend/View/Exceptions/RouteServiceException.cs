namespace View.Exceptions;

public class RouteServiceException : Exception {
	public RouteServiceException(string message, Exception inner)
		: base(message, inner) { }
}
