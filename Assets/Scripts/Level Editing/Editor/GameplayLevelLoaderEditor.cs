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

		//Check to see if there are any new values in the enum
		foreach(var value in Enum.GetValues(typeof(CubeTypes)))
		{
			if ((int)value > gll.CubePrefabs.Count )
			{
				gll.CubePrefabs.Insert((int)value, null);
			}
		}
		//for (int i  = 0; i < gll.CubePrefabs.Count; i++)
		//{
		//	if ( !Enum.IsDefined(typeof(CubeTypes), i) )
		//		gll.CubePrefabs.RemoveAt(i);
		//}

		gll.isMapExpanded = EditorGUILayout.Foldout( gll.isMapExpanded, "Cube Type Prefabs");

		if ( gll.isMapExpanded )
		{

			var cubeTypes = Enum.GetValues(typeof(CubeTypes));
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
