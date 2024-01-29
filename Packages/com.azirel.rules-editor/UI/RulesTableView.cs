using Azirel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UnityEngine.UIElements
{
	public class RulesTableView : MultiColumnListView
	{
		private const string titleTemplateGUID = "7af210c69ac695c46bf76f62fff5a159";
		private readonly VisualTreeAsset ruleTitleElement;

		private IList<AnalyzerRule> rules = Enumerable.Empty<AnalyzerRule>().ToList();

		public RulesTableView()
			=> ruleTitleElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(titleTemplateGUID));

		public void UpdateItemSource(IList<AnalyzerRule> rules)
		{
			this.rules = rules;
			itemsSource = rules as IList;
			Rebuild();
		}

		private void MapCellsBindings()
		{
			columns["ID"].makeCell = () => new Label();
			columns["Category"].makeCell = () => new Label();
			columns["Title"].makeCell = () => ruleTitleElement.Instantiate();
			columns["Severity"].makeCell = () => new SeverityField();
			columns["ID"].bindCell = BindIdCell;
			columns["Category"].bindCell = BindCategory;
			columns["Title"].bindCell = BindTitleCell;
			columns["Title"].stretchable = true;
			columns["Severity"].bindCell = BindSeverityCell;
			columns["Severity"].unbindCell = UnbindSeverityCell;
		}

		private void BindCategory(VisualElement element, int itemIndex)
			=> (element as Label).text = rules[itemIndex].Descriptor.Category;

		private void UnbindSeverityCell(VisualElement element, int arg2)
			=> (element as SeverityField)?.UnbindRule();

		private void BindSeverityCell(VisualElement element, int itemIndex)
		{
			var enumField = element as SeverityField;
			var rule = rules[itemIndex];
			enumField.BindRule(rule);
			_ = enumField.RegisterValueChangedCallback(HandleSelectedElementsSeverityChange);
		}

		private void HandleSelectedElementsSeverityChange(ChangeEvent<Enum> evt)
		{
			foreach (var selectedIndex in selectedIndices)
			{
				var rule = rules[selectedIndex];
				rule.Severity = Enum.Parse<DiagnosticSeverity>(evt.newValue.ToString());
			}
		}

		private void BindTitleCell(VisualElement element, int itemIndex)
		{
			(element.Q("TitleDescriptionFoldout") as Foldout).text = rules[itemIndex].Descriptor.Title;
			(element.Q(name: "Description") as TextField).value = rules[itemIndex].Descriptor.Description;
		}

		private void BindIdCell(VisualElement element, int itemIndex)
		{
			var rule = rules[itemIndex];
			var label = element as Label;
			label.text = String.IsNullOrEmpty(rule.Descriptor.HelpLinkUri)
				? rule.Id : $"<a href=\"{rule.Descriptor.HelpLinkUri}\">{rule.Id}</a>";
		}

		public new class UxmlFactory : UxmlFactory<RulesTableView, UxmlTraits>
		{
			public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
			{
				var result = base.Create(bag, cc) as RulesTableView;
				result.MapCellsBindings();
				return result;
			}
		}
	}
}
