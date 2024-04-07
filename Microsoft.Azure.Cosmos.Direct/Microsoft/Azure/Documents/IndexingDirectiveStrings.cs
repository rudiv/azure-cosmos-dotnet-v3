using System;

namespace Microsoft.Azure.Documents;

internal static class IndexingDirectiveStrings
{
	public static readonly string Default = IndexingDirective.Default.ToString();

	public static readonly string Include = IndexingDirective.Include.ToString();

	public static readonly string Exclude = IndexingDirective.Exclude.ToString();

	public static string FromIndexingDirective(IndexingDirective directive)
	{
		return directive switch
		{
			IndexingDirective.Default => Default, 
			IndexingDirective.Exclude => Exclude, 
			IndexingDirective.Include => Include, 
			_ => throw new ArgumentException($"Missing indexing directive string for {directive}"), 
		};
	}
}
