﻿using System;
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
		public event Action<string> SaveByPath;

		private string ruleSetsSearchFilter = "glob:\"Assets/**.ruleset\"";

		public void UpdateItems()
			=> itemsSource = AssetDatabase.FindAssets(ruleSetsSearchFilter)
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.ToList();

		public void Init()
		{
			columns["Path"].makeCell = () => new Label();
			columns["Load"].makeCell = () => new Button();
			columns["Save"].makeCell = () => new Button();

			columns["Path"].bindCell = BindPathCell;
			columns["Load"].bindCell = BindLoadCell;
			columns["Save"].bindCell = BindSaveCell;

		}

		private void BindSaveCell(VisualElement element, int elementIndex)
		{
			var button = (element as Button);
			button.text = "Save";
			button.userData = itemsSource[elementIndex];
			button.RegisterCallback<ClickEvent>(HandleSave);
		}

		private void HandleSave(ClickEvent evt)
			=> SaveByPath?.Invoke((evt.target as Button).userData as string);


		private void BindLoadCell(VisualElement element, int elementIndex)
		{
			var button = (element as Button);
			button.text = "Load";
			button.userData = itemsSource[elementIndex];
			button.RegisterCallback<ClickEvent>(HandleLoad);
		}

		private void HandleLoad(ClickEvent evt)
			=> LoadByPath?.Invoke((evt.target as Button).userData as string);

		private void BindPathCell(VisualElement element, int itemIndex)
			=> (element as Label).text = itemsSource[itemIndex] as string;
	}
}
