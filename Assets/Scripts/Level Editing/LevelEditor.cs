using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.UI;

public class LevelEditor : ScriptableObject {

	#region Instantiation

	[MenuItem("Assets/Create/Level Editor")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<LevelEditor> ();
	}

	public LevelEditor ()
	{
		//EditorApplication.update += EditLevel;
	}

	void OnEnable()
	{
		EditorApplication.update += EditLevel;
	}
	#endregion

	#region Editor Interface

	/// <summary>
	/// If this is true, you can edit the level
	/// </summary>
	[SerializeField] private bool canEdit;

	/// <summary>
	/// The cube that will be previewed and drawn when editing.
	/// Change this as you please.
	/// </summary>
	[SerializeField] private EditorCube cubeToDraw;

	/// <summary>
	/// The transform of the object that the instantiated cubes 
	/// will be parented to
	/// </summary>
	[SerializeField] private Transform levelCubesParent;

	/// <summary>
	/// The list of cube prefabs that will be mapped to the index
	/// number of the cube type enum
	/// </summary>
	[SerializeField] private List<EditorCube> cubeTypeMap;

	/// <summary>
	/// The UGUI text element that displays the coordinates of 
	/// cube preview
	/// </summary>
	[SerializeField] private Text coordinatesText;

	#endregion

	#region Static Interface

	public static LevelContainer Instance;

	public static Dictionary<Vector3, EditorCube> LevelCubes
	{
		get
		{
			if ( levelCubes == null )
				levelCubes = new Dictionary<Vector3, EditorCube>();

			return levelCubes;
		}
		set
		{
			if ( levelCubes == null )
				levelCubes = new Dictionary<Vector3, EditorCube>();

			levelCubes = value;
		}
	}
	private static Dictionary<Vector3, EditorCube> levelCubes = null;

	public static void AddCube(Vector3 position, Quaternion rotation, EditorCube cubeToInstantiate, Transform cubeParent)
	{
		EditorCube newCube = GameObject.Instantiate(cubeToInstantiate, position, rotation) as EditorCube;

		EditorCube existingCube;

		if ( LevelCubes.TryGetValue(position, out existingCube) )
		{
			Destroy(existingCube.gameObject);
			LevelCubes[position] = newCube;
		}
		else
		{
			LevelCubes.Add(position, newCube);
		}

		newCube.transform.parent = cubeParent;
	}

	public static void AddCube(SaveData data, Transform cubeParent)
	{
		EditorCube cubeToInstantiate = Instance.cubeTypeMap[(int)data.cubeType];
		Vector3 position = new Vector3(data.xPosition, data.yPosition, 0f);
		//TODO: Serialize rotation as an int, 1 2 3 4 for 0, 90, etc
		EditorCube newCube = GameObject.Instantiate(cubeToInstantiate, position, Quaternion.identity) as EditorCube;

		EditorCube existingCube;

		if ( LevelCubes.TryGetValue(position, out existingCube) )
		{
			Destroy(existingCube.gameObject);
			LevelCubes[position] = newCube;
		}
		else
		{
			LevelCubes.Add(position, newCube);
		}

		newCube.transform.parent = cubeParent;
	}

	#endregion

	#region Save Load Functions

	[Serializable]
	public class SaveData
	{
		public float xPosition;
		public float yPosition;
		public CubeTypes cubeType;

		public SaveData (EditorCube cube)
		{
			this.xPosition = cube.transform.position.x;
			this.yPosition = cube.transform.position.y;
			this.cubeType = cube.Type;
		}
	}

	/// <summary>
	/// Saves the tiles to binary
	/// </summary>
	/// <param name="fileName"></param>
	public static void SaveLevel(string levelName)
	{
		SaveData[] cubesToSave = new SaveData[LevelCubes.Count];
		int i = 0;
		foreach (var cube in LevelCubes.Values)
		{
			cubesToSave[i] = new SaveData(cube);
			i++;
		}

		string path = Path.Combine(Application.persistentDataPath, levelName + ".dat");
		Debug.Log("Saving to: " + path);
		FileStream fs = new FileStream(path, FileMode.Create);

		BinaryFormatter formatter = new BinaryFormatter();
		try
		{
			formatter.Serialize(fs, cubesToSave);
			Debug.Log("Finished Saving.");
		}
		catch ( Exception e )
		{
			Debug.LogError("Failed to serialize level. Reason: " + e.Message);
			throw;
		}
		finally
		{
			fs.Close();
		}
		//callback
	}

	/// <summary>
	/// Loads the tiles
	/// </summary>
	/// <param name="fileName"></param>
	public static SaveData[] LoadGameplayLevel( string fileName )
	{
		SaveData[] cubesToLoad = null;

		string path = Path.Combine(Application.persistentDataPath, fileName + ".dat");
		FileStream fs = new FileStream(path, FileMode.Open);
		try
		{
			BinaryFormatter formatter = new BinaryFormatter();

			cubesToLoad = (SaveData[])formatter.Deserialize(fs);
			Debug.Log("Level loaded.");
		}
		catch ( Exception e )
		{
			Debug.LogError("Failed to deserialize level. Reason: " + e.Message);
		}
		finally
		{
			fs.Close();
		}

		return cubesToLoad;
	}

	#endregion

	#region Private Functions

	/// <summary>
	/// This is called every "Update" of the editor, whatever that means
	/// </summary>
	private void EditLevel()
	{
		Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if ( canEdit && Camera.main.rect.Contains(mousePoint))
		ShowCubePreview();

		if ( Input.GetMouseButtonDown(0) )
			LevelContainer.AddCube(CubeRound(cubeToDraw.transform.position.x, cubeToDraw.transform.position.y), cubeToDraw.transform.rotation, cubeToDraw, levelCubesParent);
	}

	/// <summary>
	/// Takes in an x and y position and returns a vector3
	/// of where the cube should be placed in 3D space.
	/// This is assuming that the cube's measurements is 1.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private Vector3 CubeRound(float x, float y)
	{
		Vector3 point = new Vector3(Mathf.Floor(x), Mathf.Floor(y), 0);

		coordinatesText.text = point.ToString();

		return point;
	}

	private void ShowCubePreview()
	{
		Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float xPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
		float yPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

		//transparentCube.position = new Vector3(xPosition, yPosition, 0f);
		cubeToDraw.transform.position = CubeRound(xPosition, yPosition);
	}

	private bool CheckMouseInGameView(Vector3 position)
	{
		return Camera.main.rect.Contains(position);
	}

	#endregion
}
