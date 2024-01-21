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

	private RulesTableView rulesSpreadSheetView;
	private RuleSetsTableView setsSpreadSheetView;

	[MenuItem("Tools/Multicolumn window")]
	public static new void Show()
		=> GetWindow<RulesMulticolumnEditorWindow>(nameof(RulesMulticolumnEditorWindow));

	protected void CreateGUI()
	{
		uxmlDocument.CloneTree(rootVisualElement);
		MapRulesExtractionButton();
		MapSearch();
		MapRulesToSpreadSheet();
		MapRulesets();
	}

	private void MapRulesets()
	{
		setsSpreadSheetView = rootVisualElement.Q<RuleSetsTableView>();
		setsSpreadSheetView.UpdateItems();
		setsSpreadSheetView.Init();
		var updateRuleSetsButton = rootVisualElement.Q<Button>(name: "UpdateRulesets");
		updateRuleSetsButton.RegisterCallback<ClickEvent>(UpdateRulesets);
		setsSpreadSheetView.LoadByPath += LoadFromRuleSet;
	}

	private void LoadFromRuleSet(string obj)
	{
		throw new NotImplementedException();
	}

	private void UpdateRulesets(ClickEvent evt)
		=> setsSpreadSheetView.UpdateItems();

	private void MapSearch()
	{
		var searchField = rootVisualElement.Q<TextField>(name: "Search");
		searchField.RegisterValueChangedCallback(FilterRules);
	}

	private void FilterRules(ChangeEvent<string> evt)
	{
		searchValue = evt.newValue;
		rulesSpreadSheetView.Init(rulesList);
	}

	private void MapRulesExtractionButton()
	{
		var loadButton = rootVisualElement.Q<Button>(name: "LoadAnalyzers");
		loadButton.RegisterCallback<ClickEvent>(HandleLoadRules);
	}

	private void MapRulesToSpreadSheet()
	{
		rulesSpreadSheetView = rootVisualElement.Q<RulesTableView>();
		rulesSpreadSheetView.Init(rulesList);
	}

	private void HandleLoadRules(ClickEvent _)
	{
		rules = RulesExtractor.ExtractRules();
		rulesSpreadSheetView.Init(rulesList);
	}

	public void Clear() => rulesSpreadSheetView.Clear();
}
