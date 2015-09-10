using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(LevelEditor)), CanEditMultipleObjects]
public class LevelEditorInspector : Editor {

	#region Private Fields

	[SerializeField] private int indexToAddCubeTypeMap = 0;

	#endregion

	public override void OnInspectorGUI()
	{
		LevelEditor le = (LevelEditor)target;

		EditorGUILayout.LabelField("Testing");

		le.CanEdit = EditorGUILayout.Toggle("Can Edit", le.CanEdit);

		le.CoordinatesText = EditorGUILayout.ObjectField("Coordinates Text", le.CoordinatesText, typeof(Text), true) as Text;

		le.styles = EditorGUILayout.ObjectField("Styles", le.styles, typeof(LevelEditorGUIStyles), true) as LevelEditorGUIStyles;

		le.LevelCubesParent = EditorGUILayout.ObjectField("Level Cubes Parent", le.LevelCubesParent, typeof(Transform), true) as Transform;

		///Seperator
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		if ( le.styles != null )
		{
			EditorGUILayout.BeginVertical(le.styles.verticalGroupStyle);

			le.FileName = EditorGUILayout.TextField("File Name", le.FileName);

			if (GUILayout.Button("List Level Files"))
			{
				le.DisplayLevelFileNames();
			}

			if ( GUILayout.Button("Save Level") )
			{
				if ( le.CheckForExistingFile(le.FileName) )
				{
					if ( EditorUtility.DisplayDialog("File Exists", "The file '" + le.FileName + "' already exists, do you want to overwrite it?", "Overwrite", "Cancel") )
						le.SaveLevel(le.FileName);
				}
				else
					le.SaveLevel(le.FileName);
			}

			if ( GUILayout.Button("Load Level") )
			{
				if ( EditorUtility.DisplayDialog("Load Level", "Do you want to clear out the current cubes?", "Clear 'em'", "Keep Them") )
				{
					le.LoadLevel(le.FileName, true);
				}
				else
				{
					le.LoadLevel(le.FileName, false);
				}
			}

			if ( GUILayout.Button("Clear Level") )
			{
				if ( EditorUtility.DisplayDialog("Clear Level", "Are you sure you want to remove all cubes from the level?", "Do it", "No Thank You") )
				{
					le.ClearCubes();
				}
			}

			EditorGUILayout.EndVertical();

			///Seperator
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

			le.IsTypemapExpanded = EditorGUILayout.Foldout(le.IsTypemapExpanded, "Cube Type Map");

			EditorGUILayout.BeginHorizontal();

			indexToAddCubeTypeMap = EditorGUILayout.IntField("Index", indexToAddCubeTypeMap);

			if (GUILayout.Button("+"))
			{
				if ( indexToAddCubeTypeMap < le.cubeTypeMap.Count )
					le.cubeTypeMap.Insert(indexToAddCubeTypeMap, null);
				else
					le.cubeTypeMap.Add(null);
			}

			EditorGUILayout.EndHorizontal();

			if (le.IsTypemapExpanded)
			{
				for (int i = 0; i < le.cubeTypeMap.Count; i++ )
				{
					EditorGUILayout.BeginHorizontal();

					if ( GUILayout.Button("X") )
						le.cubeTypeMap.Remove(le.cubeTypeMap[i]);

					le.cubeTypeMap[i] = EditorGUILayout.ObjectField(le.cubeTypeMap[i], typeof(LevelCubeObject), false) as LevelCubeObject;

					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}
}
