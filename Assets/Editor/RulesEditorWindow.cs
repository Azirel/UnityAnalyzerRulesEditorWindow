using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RulesEditorWindow : EditorWindow
{
	private ILookup<string, AnalyzerRule> rules = Utilities.EmptyLookup<string, AnalyzerRule>();
	//private IList rulesList => rules.SelectMany(group => group).ToList();

	private IList rulesList => new List<AnalyzerRule>()
	{
		new AnalyzerRule("id01", "test"),
		new AnalyzerRule("id02", "test"),
		new AnalyzerRule("id03", "test"),
	};

	private ListView itemsList;

	[MenuItem("Tools/RulesEditorWindow")]
	public static void ShowExample()
	{
		var window = GetWindow<RulesEditorWindow>();
		window.titleContent = new GUIContent("RulesEditorWindow");
		window.minSize = new Vector2(500, 100);
	}

	private void CreateGUI()
	{
		// Reference to the root of the window.
		var root = rootVisualElement;

		//root.styleSheets.Add(Resources.Load<StyleSheet>("RulesEditorStyle"));

		// Loads and clones our VisualTree (eg. our UXML structure) inside the root.
		var quickToolVisualTree = Resources.Load<VisualTreeAsset>("RulesEditor");
		quickToolVisualTree.CloneTree(root);

		// Queries all the buttons (via class name) in our root and passes them
		// in the SetupButton method.
		//var toolButtons = root.Query(className: "rule-item");
		//var 
		//toolButtons.ForEach(SetupRule);

		var loadButton = root.Q<Button>(name: "LoadAnalyzers");
		loadButton.RegisterCallback<ClickEvent>(HandleLoadValues);
		itemsList = root.Q<ListView>("RulesList");
		itemsList.makeItem = MakeItem;
		itemsList.bindItem = BindItem;
		itemsList.itemsSource = rulesList;
		itemsList.itemsChosen += Debug.Log;
		itemsList.selectionChanged += Debug.Log;
		itemsList.style.flexGrow = 1f;
		itemsList.Rebuild();
	}

	private void BindItem(VisualElement element, int arg2)
	{

	}

	private RuleVisualElement MakeItem() => new RuleVisualElement();

	private void HandleLoadValues(ClickEvent evt)
	{
		//rules = RulesExtractor.ExtractRules();
		itemsList.itemsSource = rulesList;
		itemsList.Rebuild();
	}
}
