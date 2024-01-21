using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace UnityEngine.UIElements
{
	public class RuleSetsTableView : MultiColumnListView
	{
		public new class UxmlFactory : UxmlFactory<RuleSetsTableView, UxmlTraits> { }

		public event Action<string> LoadByPath;

		private string ruleSetsSearchFilter = "glob:\"Assets/**.ruleset\"";
		private IEnumerable<string> ruleSetsPaths;

		public void UpdateItems()
		{
			ruleSetsPaths = AssetDatabase.FindAssets(ruleSetsSearchFilter)
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
			itemsSource = ruleSetsPaths.ToList();
		}

		public void Init()
		{
			columns["Path"].makeCell = () => new Label();
			columns["Load"].makeCell = () => new Button();
			columns["Save"].makeCell = () => new Button();

			columns["Path"].bindCell = BindPathCell;
			columns["Load"].bindCell = BindLoadCell;
			columns["Save"].bindCell = BindSaveCell;

		}

		private void BindSaveCell(VisualElement element, int arg2)
		{
			var button = (element as Button);
			button.text = "Save";
		}

		private void BindLoadCell(VisualElement element, int arg2)
		{
			var button = (element as Button);
			button.text = "Load";
			button.userData = itemsSource[arg2];
			button.RegisterCallback<ClickEvent>(HandleLoad);
		}

		private void HandleLoad(ClickEvent evt)
			=> LoadByPath?.Invoke((evt.target as Button).userData as string);

		private void BindPathCell(VisualElement element, int itemIndex)
			=> (element as Label).text = itemsSource[itemIndex] as string;
	}
}
