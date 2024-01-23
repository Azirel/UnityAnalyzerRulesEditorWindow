using System;

namespace UnityEngine.UIElements
{
	public class SeverityField : EnumField
	{
		public new class UxmlFactory : UxmlFactory<SeverityField, UxmlTraits> { }

		private AnalyzerRule rule;

		public SeverityField() : base(default(DiagnosticSeverity)) { }

		public void BindRule(AnalyzerRule rule)
		{
			this.rule = rule;
			this.RegisterValueChangedCallback(HandleValueChange);
			rule.SeverityValueChange += UpdateSeverity;
			UpdateSeverity(rule.Severity);
		}

		public void UnbindRule() => rule.SeverityValueChange -= UpdateSeverity;

		private void HandleValueChange(ChangeEvent<Enum> evt)
			=> rule.Severity = Enum.Parse<DiagnosticSeverity>(evt.newValue.ToString());

		private void UpdateSeverity(DiagnosticSeverity severity)
				=> SetValueWithoutNotify(severity);
	}
}
