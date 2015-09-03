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
		LevelEditor levelEditor = (LevelEditor)target;

		EditorGUILayout.LabelField("Testing");

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Coordinates Text");
			levelEditor.CoordinatesText = EditorGUILayout.ObjectField(levelEditor.CoordinatesText, typeof(Text), true) as Text;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Styles");
			levelEditor.styles = EditorGUILayout.ObjectField(levelEditor.styles, typeof(LevelEditorGUIStyles), true) as LevelEditorGUIStyles;
		EditorGUILayout.EndHorizontal();

		///Seperator
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		if ( levelEditor.styles != null )
		{
			EditorGUILayout.BeginVertical(levelEditor.styles.verticalGroupStyle);

			levelEditor.FileName = EditorGUILayout.TextField("File Name", levelEditor.FileName);

			if ( GUILayout.Button("Save Level") )
			{
				if ( levelEditor.CheckForExistingFile(levelEditor.FileName) )
				{
					if ( EditorUtility.DisplayDialog("File Exists", "The file '" + levelEditor.FileName + "' already exists, do you want to overwrite it?", "Overwrite", "Cancel") )
						levelEditor.SaveLevel(levelEditor.FileName);
				}
				else
					levelEditor.SaveLevel(levelEditor.FileName);
			}
			if ( GUILayout.Button("Load Level") )
			{
				if ( EditorUtility.DisplayDialog("Load Level", "Do you want to clear out the current cubes?", "Clear 'em'", "Keep Them") )
				{
					levelEditor.LoadGameplayLevel(levelEditor.FileName, true);
				}
				else
				{
					levelEditor.LoadGameplayLevel(levelEditor.FileName, false);
				}
			}

			EditorGUILayout.EndVertical();

			///Seperator
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

			levelEditor.IsTypemapExpanded = EditorGUILayout.Foldout(levelEditor.IsTypemapExpanded, "Cube Type Map");

			EditorGUILayout.BeginHorizontal();

			indexToAddCubeTypeMap = EditorGUILayout.IntField("Index", indexToAddCubeTypeMap);

			if (GUILayout.Button("+"))
			{
				if ( indexToAddCubeTypeMap < levelEditor.cubeTypeMap.Count )
					levelEditor.cubeTypeMap.Insert(indexToAddCubeTypeMap, null);
				else
					levelEditor.cubeTypeMap.Add(null);
			}

			EditorGUILayout.EndHorizontal();

			if (levelEditor.IsTypemapExpanded)
			{
				for (int i = 0; i < levelEditor.cubeTypeMap.Count; i++ )
				{
					EditorGUILayout.BeginHorizontal();

					if ( GUILayout.Button("X") )
						levelEditor.cubeTypeMap.Remove(levelEditor.cubeTypeMap[i]);

					levelEditor.cubeTypeMap[i] = EditorGUILayout.ObjectField(levelEditor.cubeTypeMap[i], typeof(LevelCubeObject), false) as LevelCubeObject;

					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}
}
