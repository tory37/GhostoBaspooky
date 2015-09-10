using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;

public class GameplayLevelLoader : MonoBehaviour {

	#region Editor Interface

	public string LevelToLoadName
	{
		get { return levelToLoadName; }
		#if UNITY_EDITOR
		set {  levelToLoadName = value; }
		#endif
	}
	[SerializeField] private string levelToLoadName;

	public Transform LevelCubesParent
	{
		get { return levelCubesParent; }
		#if UNITY_EDITOR
		set {  levelCubesParent = value; }
		#endif
	}
	[SerializeField] private Transform levelCubesParent;

	public List<LevelCubeObject> CubePrefabs
	{
		get { return cubePrefabs; }
		#if UNITY_EDITOR
		set {  cubePrefabs = value; }
		#endif
	}
	[SerializeField] private List<LevelCubeObject> cubePrefabs = new List<LevelCubeObject>();

	public bool isMapExpanded = false;

	#endregion

	#region Load Functions

	public void DisplayLevelFileNames()
	{
		string[] files = Directory.GetFiles("Assets/Serialized Level Files/");

		foreach ( string file in files )
		{
			if (!file.Contains(".meta"))
			Debug.Log("Level Name: " + file.Remove(0, 30));
		}
	}

	public LevelEditor.SaveData[] GetFileData( string fileName )
	{
		LevelEditor.SaveData[] cubesToLoad = null;

		string path = Path.Combine("Assets/Serialized Level Files/", fileName + ".dat");
		FileStream fs = new FileStream(path, FileMode.Open);
		try
		{
			BinaryFormatter formatter = new BinaryFormatter();

			cubesToLoad = (LevelEditor.SaveData[])formatter.Deserialize(fs);
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

	public void LoadLevel()
	{
		LevelEditor.SaveData[] cubesToLoad = GetFileData(levelToLoadName);
		Debug.Log(cubesToLoad.Length);

		foreach ( Transform child in levelCubesParent )
			Destroy(child);

		for ( int i = 0; i < cubesToLoad.Length; i++ )
		{
			LevelEditor.SaveData save = cubesToLoad[i];
			int index = (int)save.cubeType;
			LevelCubeObject newCube = GameObject.Instantiate(cubePrefabs[index], new Vector3(save.xPosition, save.yPosition, 0f), Quaternion.identity) as LevelCubeObject;

			newCube.transform.parent = levelCubesParent;
		}
	}

	#endregion
}
