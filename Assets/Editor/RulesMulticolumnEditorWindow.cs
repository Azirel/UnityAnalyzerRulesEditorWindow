using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RulesMulticolumnEditorWindow : EditorWindow
{
	[SerializeField] private VisualTreeAsset uxmlDocument;

	private ILookup<string, AnalyzerRule> rules = Utilities.EmptyLookup<string, AnalyzerRule>();

	private IList<AnalyzerRule> rulesList => rules.SelectMany(group => group).ToList();

	private RulesTableView speadSheet;

	[MenuItem("Tools/Multicolumn window")]
	public static new void Show()
		=> GetWindow<RulesMulticolumnEditorWindow>(nameof(RulesMulticolumnEditorWindow));

	protected void CreateGUI()
	{
		uxmlDocument.CloneTree(rootVisualElement);
		MapRulesExtractionButton();
		MapRulesToSpreadSheet();
	}

	private void MapRulesExtractionButton()
	{
		var loadButton = rootVisualElement.Q<Button>(name: "LoadAnalyzers");
		loadButton.RegisterCallback<ClickEvent>(HandleLoadRules);
	}

	private void MapRulesToSpreadSheet()
	{
		speadSheet = rootVisualElement.Q<RulesTableView>();
		speadSheet.Init(rulesList);
	}

	private void HandleLoadRules(ClickEvent _)
	{
		rules = RulesExtractor.ExtractRules();
		speadSheet.Init(rulesList);
	}
}
