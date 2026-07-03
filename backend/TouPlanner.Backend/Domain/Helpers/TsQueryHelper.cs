namespace Domain.Helpers;

public static class TsQueryHelper {
	// Builds a prefix-matching tsquery string (e.g. "ab" -> "ab:*") so partial words
	// like "ab" match stored lexemes like "abc". plainto_tsquery/PlainToTsQuery only
	// matches whole (stemmed) lexemes, so it can't do this on its own.
	public static string BuildPrefixTsQuery(string query) {
		IEnumerable<string> terms = query
			.Split([' ', '\t', '\n'], StringSplitOptions.RemoveEmptyEntries)
			.Select(term => new string(term.Where(char.IsLetterOrDigit).ToArray()))
			.Where(term => term.Length > 0);

		return string.Join(" & ", terms.Select(term => $"{term}:*"));
	}
}
