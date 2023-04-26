using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class LevelData : ScriptableObject
{

	[System.Serializable]
	public class SpawnData
	{
		public GameObject prefab;
		public string name;
		public Vector3 position;
		public Quaternion rotation;
	}

	public List<SpawnData> spawns;

#if UNITY_EDITOR

	[UnityEditor.MenuItem("Assets/Create/LevelData")]
	public static void CreateAsset()
	{
		LevelData level = CreateInstance<LevelData>();
		try {
			level.spawns = new List<SpawnData>();
			foreach (GameObject item in UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().GetRootGameObjects()) {
				if (UnityEditor.PrefabUtility.IsOutermostPrefabInstanceRoot(item)) {
					level.spawns.Add(
						new SpawnData()
						{
							prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(item),
							name = item.name,
							position = item.transform.position,
							rotation = item.transform.rotation
						}
					);
				}
			}
			if (level.spawns.Count > 0) {
				string levelName = null;
				while (string.IsNullOrWhiteSpace(levelName)) { levelName = EditorInputDialog.StringPrompt("Level Name", "Enter the name for your level", ""); }
				UnityEditor.AssetDatabase.CreateAsset(level, "Assets/Resources/Levels/" + levelName + ".asset");
			}
			else { Debug.Log("Create LevelData Failed: 0 AI Found"); }
		}
		catch (System.Exception e) { Debug.Log(e); DestroyImmediate(level); }
	}

	[ContextMenu("Load Level Additively")]
	private void Load()
	{
		foreach (SpawnData spawn in spawns) {
			GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(spawn.prefab) as GameObject;
			go.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
			go.name = spawn.name;
		}
	}

#endif

}
