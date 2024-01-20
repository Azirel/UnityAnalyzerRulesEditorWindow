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

	private IList<AnalyzerRule> rulesList => rules
		.SelectMany(group => group)
		.Where(rule => rule.Match(searchValue))
		.ToList();

	private string searchValue = string.Empty;

	private RulesTableView spreadSheet;

	[MenuItem("Tools/Multicolumn window")]
	public static new void Show()
		=> GetWindow<RulesMulticolumnEditorWindow>(nameof(RulesMulticolumnEditorWindow));

	//[MenuItem("Tools/Clear")]
	//public static void ClearContent() => spreadSheet.viewController.ClearItems();

	protected void CreateGUI()
	{
		uxmlDocument.CloneTree(rootVisualElement);
		MapRulesExtractionButton();
		MapSearch();
		MapRulesToSpreadSheet();
	}

	private void MapSearch()
	{
		var searchField = rootVisualElement.Q<TextField>(name: "Search");
		searchField.RegisterValueChangedCallback(FilterRules);
	}

	private void FilterRules(ChangeEvent<string> evt)
	{
		searchValue = evt.newValue;
		spreadSheet.Init(rulesList);
	}

	private void MapRulesExtractionButton()
	{
		var loadButton = rootVisualElement.Q<Button>(name: "LoadAnalyzers");
		loadButton.RegisterCallback<ClickEvent>(HandleLoadRules);
	}

	private void MapRulesToSpreadSheet()
	{
		spreadSheet = rootVisualElement.Q<RulesTableView>();
		spreadSheet.Init(rulesList);
	}

	private void HandleLoadRules(ClickEvent _)
	{
		rules = RulesExtractor.ExtractRules();
		spreadSheet.Init(rulesList);
	}

	public void Clear() => spreadSheet.Clear();
}
