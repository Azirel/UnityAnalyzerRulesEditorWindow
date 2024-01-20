using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	public class RulesTableView : MultiColumnListView
	{
		public new class UxmlFactory : UxmlFactory<RulesTableView, UxmlTraits> { }

		private readonly IDictionary<AnalyzerRule, EnumField> RulesToSeverityEnums = Utilities.EmptyDictionary<AnalyzerRule, EnumField>();

		private IList rules = Enumerable.Empty<object>().ToList();

		public void Init(IList<AnalyzerRule> rules)
		{
			this.rules = rules as IList;
			RulesToSeverityEnums.Clear();
			columns["ID"].makeCell = () => new Label();
			columns["Title"].makeCell = () => new Label();
			columns["Severity"].makeCell = () => new EnumField(DiagnosticSeverity.None);

			columns["ID"].bindCell = BindIdCell;
			columns["Title"].bindCell = BindTitleCell;
			columns["Severity"].bindCell = BindSeverity;
			columns["Severity"].unbindCell = UnbindSeverity;
			itemsSource = this.rules;
		}

		private void UnbindSeverity(VisualElement element, int itemIndex)
		{
			var rule = rules[itemIndex] as AnalyzerRule;
			RulesToSeverityEnums.Remove(rule);
		}

		private void BindSeverity(VisualElement element, int itemIndex)
		{
			var enumField = (element as EnumField);
			var rule = rules[itemIndex] as AnalyzerRule;
			RulesToSeverityEnums.Add(rule, enumField);
			enumField.SetValueWithoutNotify(rule.Severity);
			enumField.RegisterValueChangedCallback(HandleThisElementSeverityChange);

			enumField.RegisterValueChangedCallback(HandleSelectedElementsSeverityChange);

			void HandleThisElementSeverityChange(ChangeEvent<Enum> evt)
				=> rule.Severity = Enum.Parse<DiagnosticSeverity>(evt.newValue.ToString());
		}

		private void HandleSelectedElementsSeverityChange(ChangeEvent<Enum> evt)
		{
			foreach (var selectedIndex in selectedIndices)
			{
				var rule = rules[selectedIndex] as AnalyzerRule;
				rule.Severity = Enum.Parse<DiagnosticSeverity>(evt.newValue.ToString());
				RulesToSeverityEnums[rule].SetValueWithoutNotify(evt.newValue);
			}
		}

		private void BindTitleCell(VisualElement element, int itemIndex)
			=> (element as Label).text = (rules[itemIndex] as AnalyzerRule).Descriptor.Title;

		private void BindIdCell(VisualElement element, int itemIndex)
		{
			var rule = rules[itemIndex] as AnalyzerRule;
			var label = (element as Label);
			if (string.IsNullOrEmpty(rule.Descriptor.HelpLinkUri))
				label.text = rule.Id;
			else
				label.text = $"<a href=\"{rule.Descriptor.HelpLinkUri}\">{rule.Id}</a>";
		}
	}
}
