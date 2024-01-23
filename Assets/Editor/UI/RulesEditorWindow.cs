using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azirel
{
	public class RulesEditorWindow : EditorWindow
	{
		[SerializeField] private VisualTreeAsset uxmlDocument;
		private string searchValue = String.Empty;
		private RulesTableView rulesSpreadSheetView;
		private RuleSetsTableView setsSpreadSheetView;

		private ILookup<string, AnalyzerRule> rulesMainSource = Utilities.EmptyLookup<string, AnalyzerRule>();

		private IList<AnalyzerRule> unfilteredRulesList => rulesMainSource
			.SelectMany(group => group)
			.ToList();

		private IList<AnalyzerRule> filteredRulesList => rulesMainSource
			.SelectMany(group => group)
			.Where(rule => rule.Match(searchValue))
			.ToList();

		[MenuItem("Tools/Rules Editor ")]
		public static new void Show()
			=> GetWindow<RulesEditorWindow>(nameof(RulesEditorWindow));

		[MenuItem("Tools/Load cached")]
		public static void LoadCachedRules()
			=> GetWindow<RulesEditorWindow>().rulesMainSource = RulesExtractor.GetCachedRules();

		protected void CreateGUI()
		{
			uxmlDocument.CloneTree(rootVisualElement);
			rulesMainSource = RulesExtractor.GetCachedRules();
			MapRulesExtractionButton();
			MapSearch();
			MapRulesToSpreadSheet();
			MapRulesets();
		}

		private void OnEnable() => searchValue = String.Empty;

		private void MapRulesets()
		{
			setsSpreadSheetView = rootVisualElement.Q<RuleSetsTableView>();
			setsSpreadSheetView.UpdateItems();
			setsSpreadSheetView.Init();
			var updateRuleSetsButton = rootVisualElement.Q<Button>(name: "UpdateRulesets");
			updateRuleSetsButton.RegisterCallback<ClickEvent>(HandleUpdateRulesets);
			setsSpreadSheetView.LoadByPath += HandleLoadFromRuleSet;
			setsSpreadSheetView.SaveByPath += HandleSaveToRuleSet;
		}

		private void HandleSaveToRuleSet(string ruleSetPath)
			=> RulesetIO.SaveTo(ruleSetPath, rulesMainSource);

		private void HandleLoadFromRuleSet(string path)
		{
			var loadedRules = RulesetIO.LoadRulesetFile(path).ToList();
			if (unfilteredRulesList.Any())
			{
				var loadedSeverities = unfilteredRulesList
					.Join(loadedRules, rule => rule.Id, rule => rule.id,
					(currentRule, loadedValue) => (currentRule, loadedValue));
				foreach (var (currentRule, loadedValue) in loadedSeverities)
					currentRule.Severity = loadedValue.severity;
			}
		}

		private void HandleUpdateRulesets(ClickEvent evt)
			=> setsSpreadSheetView.UpdateItems();

		private void MapSearch()
		{
			var searchField = rootVisualElement.Q<TextField>(name: "Search");
			_ = searchField.RegisterValueChangedCallback(HandleFilterChange);
		}

		private void HandleFilterChange(ChangeEvent<string> evt)
		{
			searchValue = evt.newValue;
			rulesSpreadSheetView.Init(filteredRulesList);
		}

		private void MapRulesExtractionButton()
		{
			var loadButton = rootVisualElement.Q<Button>(name: "LoadAnalyzers");
			loadButton.RegisterCallback<ClickEvent>(HandleLoadRules);
		}

		private void MapRulesToSpreadSheet()
		{
			rulesSpreadSheetView = rootVisualElement.Q<RulesTableView>();
			rulesSpreadSheetView.Init(filteredRulesList);
		}

		private void HandleLoadRules(ClickEvent _) => ExtractAndCacheRules();

		private void ExtractAndCacheRules()
		{
			rulesMainSource = RulesExtractor.ExtractRules();
			rulesSpreadSheetView.Init(filteredRulesList);
			RulesExtractor.CacheRules(rulesMainSource);
		}
	}
}
