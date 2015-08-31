using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorGUIWindow : EditorWindow
{
    string levelName;

	#region GUI

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Ghosto/Level Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(EditorGUIWindow));
    }
    
    void OnGUI()
    {
		GUILayout.Label("Level Editor", EditorStyles.boldLabel);
		levelName = EditorGUILayout.TextField("Level Name", levelName);

		if (GUILayout.Button("Save Level"))
		{
			LevelContainer.SaveLevel(levelName);
		}
			
		if (GUILayout.Button("Load Level"))
		{
			LevelContainer.SaveData[] levelCubes = LevelContainer.LoadGameplayLevel(levelName);

			for (int i = 0; i < levelCubes.Length; i++ )
			{
				LevelContainer.AddCube(levelCubes[i], GameObject.Find("Level Cubes").transform);
			}
		}
	}

	#endregion
}
