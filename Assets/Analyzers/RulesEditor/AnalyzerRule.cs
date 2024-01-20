public class AnalyzerRule
{
	public readonly string Id;
	public readonly DiagnosticDescriptorEssentials Descriptor;
	public readonly string AnalyzerId;
	public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Hidden;

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

	public void DrawRule() { }
}
