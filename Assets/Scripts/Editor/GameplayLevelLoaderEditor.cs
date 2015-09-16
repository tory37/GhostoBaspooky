using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GameplayLevelLoader))]
public class GameplayLevelLoaderEditor : Editor {

	public override void OnInspectorGUI()
	{
		GameplayLevelLoader gll = (GameplayLevelLoader)target;

		gll.LevelToLoadName = EditorGUILayout.TextField("Level File Name", gll.LevelToLoadName);
		gll.LevelCubesParent = EditorGUILayout.ObjectField("Level Cubes Parent", gll.LevelCubesParent, typeof(Transform), true) as Transform;

		var cubeTypes = Enum.GetValues(typeof(CubeTypes));

		while ( gll.CubePrefabs.Count < cubeTypes.Length )
			gll.CubePrefabs.Add(null);

		while ( gll.CubePrefabs.Count > cubeTypes.Length )
			gll.CubePrefabs.RemoveAt(gll.CubePrefabs.Count - 1);

		gll.isMapExpanded = EditorGUILayout.Foldout( gll.isMapExpanded, "Cube Type Prefabs");

		if (GUILayout.Button("List Files"))
		{
			gll.DisplayLevelFileNames();
		}

		if (GUILayout.Button("Load Level"))
		{
			gll.LoadLevel();
		}

		if ( gll.isMapExpanded )
		{

			for ( int i = 0; i < gll.CubePrefabs.Count; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(cubeTypes.GetValue(i).ToString());
				gll.CubePrefabs[i] = EditorGUILayout.ObjectField(gll.CubePrefabs[i], typeof(LevelCubeObject), false) as LevelCubeObject;
				EditorGUILayout.EndHorizontal();
			}
        }
	}

}
