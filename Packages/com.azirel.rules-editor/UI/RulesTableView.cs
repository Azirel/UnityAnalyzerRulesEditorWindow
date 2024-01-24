using Azirel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	public class RulesTableView : MultiColumnListView
	{
		private IList<AnalyzerRule> rules = Enumerable.Empty<AnalyzerRule>().ToList();

		public void Init(IList<AnalyzerRule> rules)
		{
			MapCellsBindings();
			this.rules = rules;
			itemsSource = rules as IList;
		}

		private void MapCellsBindings()
		{
			columns["ID"].makeCell = () => new Label();
			columns["Title"].makeCell = () => new Label();
			columns["Severity"].makeCell = () => new SeverityField();
			columns["ID"].bindCell = BindIdCell;
			columns["Title"].bindCell = BindTitleCell;
			columns["Severity"].bindCell = BindSeverityCell;
			columns["Severity"].unbindCell = UnbindSeverityCell;
		}

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
			=> (element as Label).text = rules[itemIndex].Descriptor.Title;

		private void BindIdCell(VisualElement element, int itemIndex)
		{
			var rule = rules[itemIndex];
			var label = (element as Label);
			label.text = String.IsNullOrEmpty(rule.Descriptor.HelpLinkUri)
				? rule.Id : $"<a href=\"{rule.Descriptor.HelpLinkUri}\">{rule.Id}</a>";
		}

		public new class UxmlFactory : UxmlFactory<RulesTableView, UxmlTraits> { }
	}
}
