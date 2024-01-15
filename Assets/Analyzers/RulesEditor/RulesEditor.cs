using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class RulesEditor : EditorWindow
{
	private IEnumerable<DiagnosticDescriptorEssentials> extractedDescriptors = Enumerable.Empty<DiagnosticDescriptorEssentials>();
	private Vector2 scrollPosition;

	[MenuItem("Tools/RulesEditor")]
	public static void ShowWindow() => GetWindow(typeof(RulesEditor));

	[MenuItem("Tools/DebugMethod")]
	public static void DebugMethod() => _ = RulesetLoader.LoadRulesetFile();

	protected void OnGUI()
	{
		if (GUILayout.Button(nameof(RulesExtractor.ExtractDescriptors)))
			extractedDescriptors = RulesExtractor.ExtractDescriptors();

		if (extractedDescriptors.Any())
		{
			var labelStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			foreach (var descriptor in extractedDescriptors)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(descriptor.Id, GUILayout.Width(70));
				GUILayout.Label(descriptor.Title, labelStyle, GUILayout.Width(300), GUILayout.ExpandWidth(false));
				GUILayout.Label(descriptor.Category);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}	
}