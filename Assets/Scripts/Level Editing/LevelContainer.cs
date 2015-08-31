using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelContainer : MonoBehaviour {

	#region Editor Interface

	public List<EditorCube> cubeTypeMap;

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

	#region Mono Methods

	void Start()
	{
		Instance = this;
	}

	#endregion
}
