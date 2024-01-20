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

		private IList<AnalyzerRule> rules = Enumerable.Empty<AnalyzerRule>().ToList();

		public void Init(IList<AnalyzerRule> rules)
		{
			RulesToSeverityEnums.Clear();
			columns["ID"].makeCell = () => new Label();
			columns["Title"].makeCell = () => new Label();
			columns["Severity"].makeCell = () => new EnumField(DiagnosticSeverity.None);

			columns["ID"].bindCell = BindIdCell;
			columns["Title"].bindCell = BindTitleCell;
			columns["Severity"].bindCell = BindSeverity;
			columns["Severity"].unbindCell = UnbindSeverity;
			this.rules = rules;
			itemsSource = rules as IList;
		}

		private void UnbindSeverity(VisualElement element, int itemIndex)
		{
			if (itemsSource?.Count > itemIndex)
			{
				var rule = itemsSource[itemIndex] as AnalyzerRule;
				RulesToSeverityEnums.Remove(rule);
			}
		}

		private void BindSeverity(VisualElement element, int itemIndex)
		{
			var enumField = (element as EnumField);
			var rule = rules[itemIndex];
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
				var rule = rules[selectedIndex];
				rule.Severity = Enum.Parse<DiagnosticSeverity>(evt.newValue.ToString());
				if (RulesToSeverityEnums.ContainsKey(rule))
					RulesToSeverityEnums[rule].SetValueWithoutNotify(evt.newValue);
			}
		}

		private void BindTitleCell(VisualElement element, int itemIndex)
			=> (element as Label).text = rules[itemIndex].Descriptor.Title;

		private void BindIdCell(VisualElement element, int itemIndex)
		{
			var rule = rules[itemIndex];
			var label = (element as Label);
			label.text = string.IsNullOrEmpty(rule.Descriptor.HelpLinkUri)
				? rule.Id : $"<a href=\"{rule.Descriptor.HelpLinkUri}\">{rule.Id}</a>";
		}
	}
}
