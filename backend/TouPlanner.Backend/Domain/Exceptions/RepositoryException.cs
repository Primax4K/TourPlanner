namespace Domain.Exceptions;

public class RepositoryException : Exception {
	public RepositoryException(string message, Exception inner)
		: base(message, inner) { }
}
