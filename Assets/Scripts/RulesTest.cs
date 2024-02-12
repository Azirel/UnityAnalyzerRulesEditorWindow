using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RulesTest : MonoBehaviour
{
	[MenuItem("Tools/Clear editorprefs")]
	public static void ClearEditorPrefs()
		=> EditorPrefs.DeleteAll();
}
