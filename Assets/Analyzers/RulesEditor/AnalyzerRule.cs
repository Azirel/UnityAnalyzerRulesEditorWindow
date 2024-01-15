using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class AnalyzerRule
{
	public readonly string Id;
	public readonly DiagnosticDescriptorEssentials Descriptor;
	public readonly string AnalyzerId;
	public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Hidden;

	private readonly GUIStyle labelStyle = new(GUI.skin.label) { wordWrap = true };
	private bool isUnfolded;
	private GUIStyle urlStyle = new GUIStyle(GUI.skin.label);

	public AnalyzerRule(string id, string analyzerId, DiagnosticSeverity severity = DiagnosticSeverity.None)
	{
		Id = id;
		Severity = severity;
		AnalyzerId = analyzerId;
		urlStyle.normal.textColor = new Color(0.4f, 0.5f, 0.7f);
	}

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId) : this(id: descriptor.Id, analyzerId: analyzerId)
		=> Descriptor = descriptor;

	public AnalyzerRule(DiagnosticDescriptorEssentials descriptor, string analyzerId, DiagnosticSeverity severity) : this(descriptor, analyzerId)
		=> Severity = severity;

	public void DrawRule()
	{
		GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(5));
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		if (!String.IsNullOrEmpty(Descriptor.HelpLinkUri))
		{
			if (GUILayout.Button(Descriptor.Id, urlStyle, GUILayout.Width(70)))
				Application.OpenURL(Descriptor.HelpLinkUri);
		}
		else
			GUILayout.Label(Descriptor.Id, GUILayout.Width(70));
		GUILayout.Label(Descriptor.Title, labelStyle, GUILayout.Width(300), GUILayout.ExpandWidth(false));
		GUILayout.Label(Descriptor.Category, labelStyle, GUILayout.ExpandHeight(true), GUILayout.Width(150), GUILayout.ExpandWidth(false));
		Severity = (DiagnosticSeverity)EditorGUILayout.EnumPopup(String.Empty, Severity, GUILayout.Width(70));
		if (GUILayout.Button(isUnfolded ? "Fold" : "Unfold", GUILayout.Width(60), GUILayout.ExpandWidth(false)))
			isUnfolded = !isUnfolded;
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		if (isUnfolded)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(90);
			GUI.enabled = false;
			GUILayout.TextArea(Descriptor.Description, GUILayout.ExpandHeight(true), GUILayout.Width(600));
			GUI.enabled = true;
			GUILayout.EndHorizontal();
		}
	}
}
