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
	SerializedProperty playerFront, playerBack, playerLeft, playerRight;
	SerializedProperty shape, active, size, position;
	bool rallyFoldoutState, fallbackFoldoutState, general1FoldoutState, general2FoldoutState, general3FoldoutState, general4FoldoutState, support1FoldoutState, support2FoldoutState, support3FoldoutState, support4FoldoutState, playerFrontFoldoutState, playerBackFoldoutState, playerLeftFoldoutState, playerRightFoldoutState, player5FoldoutState, player6FoldoutState, player7FoldoutState, player8FoldoutState;

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
		playerFront = serializedObject.FindProperty("playerFront");
		playerBack = serializedObject.FindProperty("playerBack");
		playerLeft = serializedObject.FindProperty("playerLeft");
		playerRight = serializedObject.FindProperty("playerRight");
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
		playerFrontFoldoutState = EditorPrefs.GetBool("zoneManagerPlayer1FoldoutState", false);
		playerBackFoldoutState = EditorPrefs.GetBool("zoneManagerPlayer2FoldoutState", false);
		playerLeftFoldoutState = EditorPrefs.GetBool("zoneManagerPlayer3FoldoutState", false);
		playerRightFoldoutState = EditorPrefs.GetBool("zoneManagerPlayer4FoldoutState", false);
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
		EditorPrefs.SetBool("zoneManagerPlayer1FoldoutState", playerFrontFoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer2FoldoutState", playerBackFoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer3FoldoutState", playerLeftFoldoutState);
		EditorPrefs.SetBool("zoneManagerPlayer4FoldoutState", playerRightFoldoutState);
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

		shape = playerFront.FindPropertyRelative("shape");	active = playerFront.FindPropertyRelative("active");
		size = playerFront.FindPropertyRelative("size");	position = playerFront.FindPropertyRelative("position");
		playerFrontFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(playerFrontFoldoutState, "PlayerFront");
		if (playerFrontFoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = playerBack.FindPropertyRelative("shape");	active = playerBack.FindPropertyRelative("active");
		size = playerBack.FindPropertyRelative("size");	position = playerBack.FindPropertyRelative("position");
		playerBackFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(playerBackFoldoutState, "PlayerBack");
		if (playerBackFoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = playerLeft.FindPropertyRelative("shape");	active = playerLeft.FindPropertyRelative("active");
		size = playerLeft.FindPropertyRelative("size");	position = playerLeft.FindPropertyRelative("position");
		playerLeftFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(playerLeftFoldoutState, "PlayerLeft");
		if (playerLeftFoldoutState) {
			GUILayout.BeginHorizontal(); 
			EditorGUILayout.LabelField("Active", GUILayout.Width(60f)); EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(32f)); 
			EditorGUILayout.LabelField("Shape", GUILayout.Width(60f)); EditorGUILayout.PropertyField(shape, GUIContent.none, GUILayout.Width(80f)); 
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(size);
			EditorGUILayout.PropertyField(position);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		shape = playerRight.FindPropertyRelative("shape");	active = playerRight.FindPropertyRelative("active");
		size = playerRight.FindPropertyRelative("size");	position = playerRight.FindPropertyRelative("position");
		playerRightFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(playerRightFoldoutState, "PlayerRight");
		if (playerRightFoldoutState) {
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
		if (playerFrontFoldoutState && playerFront.FindPropertyRelative("active").boolValue) {
			position = playerFront.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P1");
		}
		if (playerBackFoldoutState && playerBack.FindPropertyRelative("active").boolValue) {
			position = playerBack.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P2");
		}
		if (playerLeftFoldoutState && playerLeft.FindPropertyRelative("active").boolValue) {
			position = playerLeft.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P3");
		}
		if (playerRightFoldoutState && playerRight.FindPropertyRelative("active").boolValue) {
			position = playerRight.FindPropertyRelative("position");
			position.vector2Value = Handles.FreeMoveHandle(position.vector2Value, 0.5f, Vector3.zero, Handles.DotHandleCap);
			Handles.Label(position.vector2Value + labelOffset, "P4");
		}
		serializedObject.ApplyModifiedProperties();
	}

}
