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

	//private ILookup<string, AnalyzerRule> rules = Utilities.Empty<string, AnalyzerRule>();

	//private IList rulesList => rules.SelectMany(group => group).ToList();

	private IList rulesList => new List<AnalyzerRule>()
	{
		new AnalyzerRule("id01", "test"),
		new AnalyzerRule("id02", "test"),
		new AnalyzerRule("id03", "test"),
	};

	[MenuItem("Tools/Multicolumn window")]
	public static void Show()
		=> GetWindow<RulesMulticolumnEditorWindow>(nameof(RulesMulticolumnEditorWindow));

	protected void CreateGUI()
	{
		uxmlDocument.CloneTree(rootVisualElement);
		var listView = rootVisualElement.Q<MultiColumnListView>();

		listView.itemsSource = rulesList;
		listView.columns["ID"].makeCell = () => new Label();
		listView.columns["Title"].makeCell = () => new Label();

		listView.columns["ID"].bindCell = BindIdCell;
		listView.columns["Title"].bindCell = BindTitleCell;

		listView.Rebuild();
	}

	private void BindTitleCell(VisualElement element, int itemIndex)
		=> (element as Label).text = (rulesList[itemIndex] as AnalyzerRule).Id;

	private void BindIdCell(VisualElement element, int itemIndex)
		=> (element as Label).text = (rulesList[itemIndex] as AnalyzerRule).AnalyzerId;
}
