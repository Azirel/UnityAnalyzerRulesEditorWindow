using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.UIElements
{
	public class RuleVisualElement : VisualElement
	{
		private bool isSelected;
		private const string normalStyleName = "rule-item-normal";
		private const string selectedStyleName = "rule-item-selected";

		private EnumField enumField;
		private DiagnosticSeverity severity;

		public event Action<DiagnosticSeverity> SeverityChanged;

		public DiagnosticSeverity Severity => severity;

		public new class UxmlFactory : UxmlFactory<RuleVisualElement>
		{
			//public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
			//{
			//	var result = base.Create(bag, cc);
			//	var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets\Editor\Resources\RuleTemplate.uxml");
			//	visualTree.CloneTree(result);
			//	return result;
			//}
		}

		public RuleVisualElement()
		{
			//var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets\Editor\Resources\RuleTemplate.uxml");
			//visualTree.CloneTree(this);
			RegisterCallback<ClickEvent>(HandleSelectionToggle);
			RegisterCallback<AttachToPanelEvent>(HandlePanelAttached);

		}

		private void HandlePanelAttached(AttachToPanelEvent evt)
		{
			enumField = this.Q<EnumField>("Severity");
			enumField?.RegisterValueChangedCallback(HandleEnumChange);
		}

		private void HandleEnumChange(ChangeEvent<Enum> changeEvent)
		{
			severity = (DiagnosticSeverity)Enum.Parse(typeof(DiagnosticSeverity), changeEvent.newValue.ToString());
			Debug.Log(severity);
		}

		public void HandleSelectionToggle(ClickEvent _)
			=> ToggleSelection();

		public void ToggleSelection()
		{
			isSelected = !isSelected;
			SetSelection(isSelected);
		}

		public void SetSelection(bool value)
		{
			if (value)
			{
				if (!ClassListContains(selectedStyleName))
					AddToClassList(selectedStyleName);
				if (ClassListContains(normalStyleName))
					RemoveFromClassList(normalStyleName);
			}
			else
			{
				if (ClassListContains(selectedStyleName))
					RemoveFromClassList(selectedStyleName);
				if (!ClassListContains(normalStyleName))
					AddToClassList(normalStyleName);
			}
		}
	}
}