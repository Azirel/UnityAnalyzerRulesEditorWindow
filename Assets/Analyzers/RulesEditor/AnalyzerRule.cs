using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class AnalyzerRule : ISearchable
{
	public readonly string Id;
	public readonly DiagnosticDescriptorEssentials Descriptor;
	public readonly string AnalyzerId;
	public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Hidden;

	private IEnumerable<string> GetSearchableContent()
	{
		yield return Id;
		yield return AnalyzerId;
		yield return Descriptor.Title;
		yield return Descriptor.Category;
		yield return Descriptor.HelpLinkUri;
		yield return Descriptor.Description;
	}

	public AnalyzerRule(string id, string analyzerId, DiagnosticSeverity severity = DiagnosticSeverity.None)
	{
		Id = id;
		Severity = severity;
		AnalyzerId = analyzerId;
	}

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId) : this(id: descriptor.Id, analyzerId: analyzerId)
		=> Descriptor = descriptor;

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId, DiagnosticSeverity severity) : this(descriptor, analyzerId)
		=> Severity = severity;

	public bool Match(string query)
		=> string.IsNullOrEmpty(query) ? true : GetSearchableContent().Any(@string => @string.Contains(query));
}
