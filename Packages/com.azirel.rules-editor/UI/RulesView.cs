using Azirel;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class RulesView : VisualElement
	{
		private TextField searchInput;
		private RulesTableView rulesTableView;
		private Button updateRulesButton;

		private TextField SearchInput
		{
			get => searchInput ??= this.Q<TextField>();
			set => searchInput = value;
		}

		private RulesTableView RulesTableView
		{
			get => rulesTableView ??= this.Q<RulesTableView>();
			set => rulesTableView = value;
		}

		private Button UpdateRulesButton
		{
			get => updateRulesButton ??= this.Q<Button>();
			set => updateRulesButton = value;
		}

		public void RegisterRulesUpdateButtonCallback(EventCallback<ClickEvent> callback)
			=> UpdateRulesButton.RegisterCallback(callback);

		public bool RegisterSearchValueChangedCallback(EventCallback<ChangeEvent<string>> callback)
			=> SearchInput.RegisterValueChangedCallback(callback);

		public void UpdateItemSource(IList<AnalyzerRule> rules)
			=> RulesTableView.UpdateItemSource(rules);

		public new class UxmlFactory : UxmlFactory<RulesView, UxmlTraits> { }
	}
}
