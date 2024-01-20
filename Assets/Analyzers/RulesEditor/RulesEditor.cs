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
	[System.NonSerialized]
	private static ILookup<string, AnalyzerRule> extractedAnalyzers;
	private Vector2 scrollPosition;
	private GUIStyle analyzerNameLabelStyle = GUIStyle.none;

	[MenuItem("Tools/RulesEditor")]
	public static void ShowWindow() => GetWindow(typeof(RulesEditor));

	[MenuItem("Tools/DebugMethod")]
	public static void DebugMethod() => _ = RulesetLoader.LoadRulesetFile();

	protected void OnEnable()
	{
		analyzerNameLabelStyle = new GUIStyle() { fontStyle = FontStyle.Bold, };
		analyzerNameLabelStyle.normal.textColor = Color.white;
		if (extractedAnalyzers?.Any() != true)
			extractedAnalyzers = RulesExtractor.GetCachedRules();
	}

	protected void OnGUI()
	{
		if (GUILayout.Button(nameof(RulesExtractor.ExtractRules)))
		{
			extractedAnalyzers = RulesExtractor.ExtractRules();
			RulesExtractor.CacheRules(extractedAnalyzers);
		}

		if (extractedAnalyzers?.Any() == true)
		{
			var labelStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			foreach (var analyzer in extractedAnalyzers)
			{
				GUILayout.Label(analyzer.Key, analyzerNameLabelStyle);
				//foreach (var rule in analyzer)
				//	rule.DrawRule();
				GUILayout.Space(20);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}
}