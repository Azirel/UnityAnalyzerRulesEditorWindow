using UnityEngine;

public class AnalyzerRule
{
	public readonly string Id;
	public readonly DiagnosticDescriptorEssentials Descriptor;
	public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Hidden;

	private readonly GUIStyle labelStyle = new(GUI.skin.label) { wordWrap = true };

	public AnalyzerRule(string id, DiagnosticSeverity severity)
	{
		Id = id;
		Severity = severity;
	}

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor)
		=> Descriptor = descriptor;

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, DiagnosticSeverity severity)
	{
		Id = descriptor.Id;
		Descriptor = descriptor;
		Severity = severity;
	}

	public void DrawRule()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(Descriptor.Id, GUILayout.Width(70));
		GUILayout.Label(Descriptor.Title, labelStyle, GUILayout.Width(300), GUILayout.ExpandWidth(false));
		GUILayout.Label(Descriptor.Category);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
