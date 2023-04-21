using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(ZoneManager))]
public class ZoneManagerEditor : Editor
{

	SerializedProperty visualize;
	SerializedProperty rally, fallback;
	SerializedProperty general1, general2, general3, general4;
	SerializedProperty support1, support2, support3, support4;
	SerializedProperty player1, player2, player3, player4, player5, player6, player7, player8;
	SerializedProperty shape, active, size, position;
	bool rallyFoldoutState, fallbackFoldoutState, general1FoldoutState, general2FoldoutState, general3FoldoutState, general4FoldoutState, support1FoldoutState, support2FoldoutState, support3FoldoutState, support4FoldoutState, player1FoldoutState, player2FoldoutState, player3FoldoutState, player4FoldoutState, player5FoldoutState, player6FoldoutState, player7FoldoutState, player8FoldoutState;

	private void OnEnable()
	{
		GetFoldoutStates();
		visualize = serializedObject.FindProperty("visualize");
		rally = serializedObject.FindProperty("rally");
		fallback = serializedObject.FindProperty("fallback");
		general1 = serializedObject.FindProperty("general1");
		general2 = serializedObject.FindProperty("general2");
		general3 = serializedObject.FindProperty("general3");
		general4 = serializedObject.FindProperty("general4");
		support1 = serializedObject.FindProperty("support1");
		support2 = serializedObject.FindProperty("support2");
		support3 = serializedObject.FindProperty("support3");
		support4 = serializedObject.FindProperty("support4");
		player1 = serializedObject.FindProperty("player1");
		player2 = serializedObject.FindProperty("player2");
		player3 = serializedObject.FindProperty("player3");
		player4 = serializedObject.FindProperty("player4");
		player5 = serializedObject.FindProperty("player5");
		player6 = serializedObject.FindProperty("player6");
		player7 = serializedObject.FindProperty("player7");
		player8 = serializedObject.FindProperty("player8");
	}
	private void OnDisable() { SaveFoldoutStates(); }

	private void GetFoldoutStates()
	{
		rallyFoldoutState = EditorPrefs.GetBool("zoneManagerRallyFoldoutState", false);
		fallbackFoldoutState = EditorPrefs.GetBool("zoneManagerFallbackFoldoutState", false);
		general1FoldoutState = EditorPrefs.GetBool("zoneManagerGeneral1FoldoutState", false);
		general2FoldoutState = EditorPrefs.GetBool("zoneManagerGeneral2FoldoutState", false);
		general3FoldoutState = EditorPrefs.GetBool("zoneManagerGeneral3FoldoutState", false);
		general4FoldoutState = EditorPrefs.GetBool("zoneManagerGeneral4FoldoutState", false);
		support1FoldoutState = EditorPrefs.GetBool("zoneManagerSupport1FoldoutState", false);
		support2FoldoutState = EditorPrefs.GetBool("zoneManagerSupport2FoldoutState", false);
		support3FoldoutState = EditorPrefs.GetBool("zoneManagerSupport3FoldoutState", false);
		support4FoldoutState = EditorPrefs.GetBool("zoneManagerSupport4FoldoutState", false);
		player1FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer1FoldoutState", false);
		player2FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer2FoldoutState", false);
		player3FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer3FoldoutState", false);
		player4FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer4FoldoutState", false);
		player5FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer5FoldoutState", false);
		player6FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer6FoldoutState", false);
		player7FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer7FoldoutState", false);
		player8FoldoutState = EditorPrefs.GetBool("zoneManagerPlayer8FoldoutState", false);
	}
	private void SaveFoldoutStates()
	{
		EditorPrefs.SetBool("zoneManagerRallyFoldoutState", rallyFoldoutState);
		EditorPrefs.SetBool("zoneManagerFallbackFoldoutState", fallbackFoldoutState);
		EditorPrefs.SetBool("zoneManagerGeneral1FoldoutState", general1FoldoutState);
		EditorPrefs.SetBool("zoneManagerGeneral2FoldoutState", general2FoldoutState);
		EditorPrefs.SetBool("zoneManagerGeneral3FoldoutState", general3FoldoutState);
		EditorPrefs.SetBool("zoneManagerGeneral4FoldoutState", general4FoldoutState);
		EditorPrefs.SetBool("zoneManagerSupport1FoldoutState", support1FoldoutState);
		EditorPrefs.SetBool("zoneManagerSupport2FoldoutState", support2FoldoutState);
		EditorPrefs.SetBool("zoneManagerSupport3FoldoutState", support3FoldoutState);
		EditorPrefs.SetBool("zoneManagerSupport4FoldoutState", support4FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer1FoldoutState", player1FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer2FoldoutState", player2FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer3FoldoutState", player3FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer4FoldoutState", player4FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer5FoldoutState", player5FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer6FoldoutState", player6FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer7FoldoutState", player7FoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer8FoldoutState", player8FoldoutState);
	}

	public override void OnInspectorGUI()
	{
		
		serializedObject.Update();

		EditorGUILayout.PropertyField(visualize);

		shape = rally.FindPropertyRelative("shape");	active = rally.FindPropertyRelative("active");
		size = rally.FindPropertyRelative("size");		position = rally.FindPropertyRelative("position");
		rallyFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(rallyFoldoutState, "Rally");
		if (rallyFoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f));
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = fallback.FindPropertyRelative("shape");	active = fallback.FindPropertyRelative("active");
		size = fallback.FindPropertyRelative("size");	position = fallback.FindPropertyRelative("position");
		fallbackFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(fallbackFoldoutState, "Fallback");
		if (fallbackFoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = general1.FindPropertyRelative("shape");	active = general1.FindPropertyRelative("active");
		size = general1.FindPropertyRelative("size");	position = general1.FindPropertyRelative("position");
		general1FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(general1FoldoutState, "General1");
		if (general1FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = general2.FindPropertyRelative("shape");	active = general2.FindPropertyRelative("active");
		size = general2.FindPropertyRelative("size");	position = general2.FindPropertyRelative("position");
		general2FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(general2FoldoutState, "General2");
		if (general2FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = general3.FindPropertyRelative("shape");	active = general3.FindPropertyRelative("active");
		size = general3.FindPropertyRelative("size");	position = general3.FindPropertyRelative("position");
		general3FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(general3FoldoutState, "General3");
		if (general3FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = general4.FindPropertyRelative("shape");	active = general4.FindPropertyRelative("active");
		size = general4.FindPropertyRelative("size");	position = general4.FindPropertyRelative("position");
		general4FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(general4FoldoutState, "General4");
		if (general4FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = support1.FindPropertyRelative("shape");	active = support1.FindPropertyRelative("active");
		size = support1.FindPropertyRelative("size");	position = support1.FindPropertyRelative("position");
		support1FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(support1FoldoutState, "Support1");
		if (support1FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = support2.FindPropertyRelative("shape");	active = support2.FindPropertyRelative("active");
		size = support2.FindPropertyRelative("size");	position = support2.FindPropertyRelative("position");
		support2FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(support2FoldoutState, "Support2");
		if (support2FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = support3.FindPropertyRelative("shape");	active = support3.FindPropertyRelative("active");
		size = support3.FindPropertyRelative("size");	position = support3.FindPropertyRelative("position");
		support3FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(support3FoldoutState, "Support3");
		if (support3FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = support4.FindPropertyRelative("shape");	active = support4.FindPropertyRelative("active");
		size = support4.FindPropertyRelative("size");	position = support4.FindPropertyRelative("position");
		support4FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(support4FoldoutState, "Support4");
		if (support4FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player1.FindPropertyRelative("shape");	active = player1.FindPropertyRelative("active");
		size = player1.FindPropertyRelative("size");	position = player1.FindPropertyRelative("position");
		player1FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player1FoldoutState, "Player1");
		if (player1FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player2.FindPropertyRelative("shape");	active = player2.FindPropertyRelative("active");
		size = player2.FindPropertyRelative("size");	position = player2.FindPropertyRelative("position");
		player2FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player2FoldoutState, "Player2");
		if (player2FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player3.FindPropertyRelative("shape");	active = player3.FindPropertyRelative("active");
		size = player3.FindPropertyRelative("size");	position = player3.FindPropertyRelative("position");
		player3FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player3FoldoutState, "Player3");
		if (player3FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player4.FindPropertyRelative("shape");	active = player4.FindPropertyRelative("active");
		size = player4.FindPropertyRelative("size");	position = player4.FindPropertyRelative("position");
		player4FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player4FoldoutState, "Player4");
		if (player4FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player5.FindPropertyRelative("shape");	active = player5.FindPropertyRelative("active");
		size = player5.FindPropertyRelative("size");	position = player5.FindPropertyRelative("position");
		player5FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player5FoldoutState, "Player5");
		if (player5FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player6.FindPropertyRelative("shape");	active = player6.FindPropertyRelative("active");
		size = player6.FindPropertyRelative("size");	position = player6.FindPropertyRelative("position");
		player6FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player6FoldoutState, "Player6");
		if (player6FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player7.FindPropertyRelative("shape");	active = player7.FindPropertyRelative("active");
		size = player7.FindPropertyRelative("size");	position = player7.FindPropertyRelative("position");
		player7FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player7FoldoutState, "Player7");
		if (player7FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = player8.FindPropertyRelative("shape");	active = player8.FindPropertyRelative("active");
		size = player8.FindPropertyRelative("size");	position = player8.FindPropertyRelative("position");
		player8FoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(player8FoldoutState, "Player8");
		if (player8FoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		serializedObject.ApplyModifiedProperties();

	}

	private Vector2 labelOffset = new Vector2(2f, 0f);
	private void OnSceneGUI()
	{
		if (!visualize.boolValue) { return; }
		if (rallyFoldoutState && rally.FindPropertyRelative("active").boolValue) {
			position = rally.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "RALLY");
		}
		if (fallbackFoldoutState && fallback.FindPropertyRelative("active").boolValue) {
			position = fallback.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "FALLBACK");
		}
		if (general1FoldoutState && general1.FindPropertyRelative("active").boolValue) {
			position = general1.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "G1");
		}
		if (general2FoldoutState && general2.FindPropertyRelative("active").boolValue) {
			position = general2.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "G2");
		}
		if (general3FoldoutState && general3.FindPropertyRelative("active").boolValue) {
			position = general3.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "G3");
		}
		if (general4FoldoutState && general4.FindPropertyRelative("active").boolValue) {
			position = general4.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "G4");
		}
		if (support1FoldoutState && support1.FindPropertyRelative("active").boolValue) {
			position = support1.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "S1");
		}
		if (support2FoldoutState && support2.FindPropertyRelative("active").boolValue) {
			position = support2.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "S2");
		}
		if (support3FoldoutState && support3.FindPropertyRelative("active").boolValue) {
			position = support3.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "S3");
		}
		if (support4FoldoutState && support4.FindPropertyRelative("active").boolValue) {
			position = support4.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "S4");
		}
		if (player1FoldoutState && player1.FindPropertyRelative("active").boolValue) {
			position = player1.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P1");
		}
		if (player2FoldoutState && player2.FindPropertyRelative("active").boolValue) {
			position = player2.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P2");
		}
		if (player3FoldoutState && player3.FindPropertyRelative("active").boolValue) {
			position = player3.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P3");
		}
		if (player4FoldoutState && player4.FindPropertyRelative("active").boolValue) {
			position = player4.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P4");
		}
		if (player5FoldoutState && player5.FindPropertyRelative("active").boolValue) {
			position = player5.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P5");
		}
		if (player6FoldoutState && player6.FindPropertyRelative("active").boolValue) {
			position = player6.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P6");
		}
		if (player7FoldoutState && player7.FindPropertyRelative("active").boolValue) {
			position = player7.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P7");
		}
		if (player8FoldoutState && player8.FindPropertyRelative("active").boolValue) {
			position = player8.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P8");
		}
		serializedObject.ApplyModifiedProperties();
	}

}
