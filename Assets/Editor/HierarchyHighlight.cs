using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyHighlight : MonoBehaviour
{

	// https://answers.unity.com/questions/1371138/how-to-change-gameobject-color-in-hierarchy-for-hi.html
	// https://unity3d.college/2017/09/04/customizing-hierarchy-bold-prefab-text/
	//private static Vector2 offset = new Vector2(0, 2);

	static HierarchyHighlight() { EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI; }

	private static GameObject go;
	private static bool initialized = false;
	private static GUIStyle currentStyle, redTextStyle, yellowTextStyle, greenTextStyle, blueTextStyle, greyTextStyle;
	private static Color backgroundColor = new Color(0.2196f, 0.2196f, 0.2196f);
	private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
	{

		if (!initialized) {
			redTextStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState() { textColor = Color.HSVToRGB(0.033f, 1f, 0.94f) } };
			yellowTextStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState() { textColor = Color.HSVToRGB(0.15f, 1f, 0.94f) } };
			greenTextStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState() { textColor = Color.HSVToRGB(0.267f, 1f, 0.94f) } };
			blueTextStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState() { textColor = Color.HSVToRGB(0.5277f, 1f, 0.94f) } };
			greyTextStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState() { textColor = new Color(0.49f, 0.49f, 0.49f) } };
			initialized = true;
		}

		Object obj = EditorUtility.InstanceIDToObject(instanceID);
		if (!obj || obj is not GameObject) { return; }
		go = obj as GameObject;

		Rect offsetRect = new Rect(selectionRect.position, selectionRect.size);

		if (go.TryGetComponent(out Module m)) {
			if (go.activeSelf) {
				if (m.parent) { currentStyle = greenTextStyle; }
				else { currentStyle = yellowTextStyle; }
			}
			else { currentStyle = redTextStyle; }
		}
		else {
			if (PrefabUtility.IsPartOfAnyPrefab(obj)) { currentStyle = blueTextStyle; }
			if (go.activeSelf) { currentStyle = EditorStyles.label; }
			else { currentStyle = greyTextStyle; }
		}
		
		EditorGUI.DrawRect(selectionRect, backgroundColor);
		EditorGUI.LabelField(offsetRect, go.name, currentStyle);

	}

}
