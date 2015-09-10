using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour {

	#region Editor Interface

	/// <summary>
	/// If this is true, you can edit the level
	/// </summary>
	public bool CanEdit
	{
		get { return canEdit; }
	#if UNITY_EDITOR
		set { canEdit = value; }
	#endif
	}
	[SerializeField] private bool canEdit;

	/// <summary>
	/// If this is true, you can edit the level
	/// </summary>
	public bool IsTypemapExpanded
	{
		get { return isTypemapExpanded; }
	#if UNITY_EDITOR
		set { isTypemapExpanded = value; }
	#endif
	}
	[SerializeField] private bool isTypemapExpanded;

	/// <summary>
	/// The cube that will be previewed and drawn when editing.
	/// Change this as you please.
	/// </summary>
	public LevelCubeObject CubeToDraw
	{
		get { return cubeToDraw; }
	#if UNITY_EDITOR
		set { cubeToDraw = value; }
	#endif
	}
	[SerializeField] private LevelCubeObject cubeToDraw;

	/// <summary>
	/// The transform of the object that the instantiated cubes 
	/// will be parented to
	/// </summary>
	public Transform LevelCubesParent
	{
		get { return levelCubesParent; }
	#if UNITY_EDITOR
		set { levelCubesParent = value; }
	#endif
	}
	[SerializeField] private Transform levelCubesParent;

	/// <summary>
	/// The list of cube prefabs that will be mapped to the index
	/// number of the cube type enum
	/// </summary>
	public List<LevelCubeObject> CubeTypeMap
	{
		get { return cubeTypeMap; }
		#if UNITY_EDITOR
		set { cubeTypeMap = value; }
		#endif
	}
	[SerializeField] public List<LevelCubeObject> cubeTypeMap;

	/// <summary>
	/// The UGUI text element that displays the coordinates of 
	/// cube preview
	/// </summary>
	public Text CoordinatesText
	{
		get { return coordinatesText; }
		#if UNITY_EDITOR
		set { coordinatesText = value; }
		#endif
	}
	[SerializeField] private Text coordinatesText;

	public string FileName;

	public LevelEditorGUIStyles styles;

	#endregion

	#region Public Interface

	public static LevelEditor Instance;

	/// <summary>
	/// A mapping of all the cubes in the level to their "exact" position
	/// in anticipation of a position being changed by miniscule float
	/// values
	/// </summary>
	public Dictionary<Vector3, LevelCubeObject> LevelCubes
	{
		get
		{
			if ( levelCubes == null )
				levelCubes = new Dictionary<Vector3, LevelCubeObject>();

			return levelCubes;
		}
		set
		{
			if ( levelCubes == null )
				levelCubes = new Dictionary<Vector3, LevelCubeObject>();

			levelCubes = value;
		}
	}
	[SerializeField] private Dictionary<Vector3, LevelCubeObject> levelCubes = null;

	/// <summary>
	/// Adds a cube to the level, saving a reference in level cube
	/// </summary>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="cubeToInstantiate"></param>
	/// <param name="cubeParent"></param>
	public void AddCube(Vector3 position, Quaternion rotation, LevelCubeObject cubeToInstantiate)
	{
		LevelCubeObject newCube = GameObject.Instantiate(cubeToInstantiate, position, rotation) as LevelCubeObject;

		LevelCubeObject existingCube;

		if ( LevelCubes.TryGetValue(position, out existingCube) )
		{
			Destroy(existingCube.gameObject);
			LevelCubes[position] = newCube;
		}
		else
		{
			LevelCubes.Add(position, newCube);
		}

		newCube.transform.parent = levelCubesParent;
	}

	/// <summary>
	/// This adds a cube by its save data, ie. when reading from 
	/// a level file
	/// </summary>
	/// <param name="data"></param>
	/// <param name="cubeParent"></param>
	public void AddCube(SaveData data)
	{
		LevelCubeObject cubeToInstantiate = cubeTypeMap[(int)data.cubeType];
		Vector3 position = new Vector3(data.xPosition, data.yPosition, 0f);
		//TODO: Serialize rotation as an int, 1 2 3 4 for 0, 90, etc
		LevelCubeObject newCube = GameObject.Instantiate(cubeToInstantiate, position, Quaternion.identity) as LevelCubeObject;

		LevelCubeObject existingCube;

		if ( LevelCubes.TryGetValue(position, out existingCube) )
		{
			Destroy(existingCube.gameObject);
			LevelCubes[position] = newCube;
		}
		else
		{
			LevelCubes.Add(position, newCube);
		}

		newCube.transform.parent = levelCubesParent;
	}

	/// <summary>
	/// Deletes a cube and removes its entry from the level cubes
	/// dictionary
	/// </summary>
	/// <param name="position"></param>
	public void RemoveCube(Vector3 position)
	{
		LevelCubeObject cubeToRemove = null;

		if ( levelCubes.TryGetValue(position, out cubeToRemove) )
		{
			Destroy(cubeToRemove);
			levelCubes.Remove(position);
		}
	}

	public void ClearCubes()
	{
		foreach ( Transform t in levelCubesParent )
			DestroyImmediate(t.gameObject);
	}

	#endregion

	#region Save Load Functions

	[Serializable]
	public class SaveData
	{
		public float xPosition;
		public float yPosition;
		public CubeTypes cubeType;

		public SaveData (LevelCubeObject cube)
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
	public void SaveLevel(string levelName)
	{
		SaveData[] cubesToSave = new SaveData[levelCubesParent.childCount];
		int i = 0;
		foreach (Transform t in levelCubesParent)
		{
			LevelCubeObject cube = t.GetComponent<LevelCubeObject>();
			if (cube != null )
				cubesToSave[i] = new SaveData(cube);
			i++;
		}

		string path = Path.Combine("Assets/Serialized Level Files/", levelName + ".dat");
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
	/// Returns true if there is a file in the directory
	/// that already exists with the name /fileName/
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public bool CheckForExistingFile( string fileName )
	{
		string path = Path.Combine("Assets/Serialized Level Files/", fileName + ".dat");
		return File.Exists(path);
	}

	/// <summary>
	/// Loads the tiles
	/// </summary>
	/// <param name="fileName"></param>
	public SaveData[] GetFileData( string fileName )
	{
		SaveData[] cubesToLoad = null;

		string path = Path.Combine("Assets/Serialized Level Files/", fileName + ".dat");
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

	public void LoadLevel(string fileName, bool clearCurrent)
	{
		SaveData[] cubesToLoad = GetFileData(fileName);

		if ( clearCurrent )
		{
			foreach ( Transform t in levelCubesParent )
				Destroy(t);
		}

		for ( int i = 0; i < cubesToLoad.Length; i++)
		{
			AddCube(cubesToLoad[i]);
		}
	}

	public void DisplayLevelFileNames()
	{
		string[] files = Directory.GetFiles("Assets/Serialized Level Files/");

		foreach ( string file in files )
		{
			if (!file.Contains(".meta"))
			Debug.Log("Level Name: " + file.Remove(0, 30));
		}
	}

	#endregion

	#region Private Functions

	/// <summary>
	/// Takes in an x and y position and returns a vector3
	/// of where the cube should be placed in 3D space.
	/// This is assuming that the cube's measurements is 1.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector3 CubeRound(Vector3 point)
	{
		Vector3 roundedPoint = new Vector3(Mathf.Floor(point.x), Mathf.Floor(point.y), 0);
		 
		return roundedPoint;
	}

	private void ShowCubePreview(Vector3 previewCubePosition)
	{
		cubeToDraw.transform.position = CubeRound(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		cubeToDraw.transform.SetPositionZ(0);
	}

	private bool CheckMouseInGameView(Vector3 position)
	{
		return Camera.main.rect.Contains(position);
	}

	private void CameraControls()
	{
		float vertical = Input.GetAxis("Vertical") * 5 * Time.deltaTime;
		float horizontal = Input.GetAxis("Horizontal") * 5 * Time.deltaTime;
		float forward = Input.GetAxis("Forward") * 5 * Time.deltaTime;

		Camera.main.transform.Translate(horizontal, vertical, 0f);

		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + forward, .5f, Mathf.Infinity);
	}

	#endregion

	private Vector3 lastPosition;

	#region Mono Methods

	void Start()
	{
		lastPosition = Vector3.zero;

		levelCubes = new Dictionary<Vector3, LevelCubeObject>();

		foreach(Transform t in levelCubesParent)
		{
			LevelCubeObject cube = t.GetComponent<LevelCubeObject>();
			if (cube != null )
				levelCubes.Add(cube.transform.position, cube);
		}
	}

	/// <summary>
	/// draws the preview of the cube then places it on click
	/// if the mouse is in the game view
	/// </summary>
	void Update()
	{
		CameraControls();

		Rect screenRect = new Rect(0,0, Screen.width, Screen.height);

		if ( canEdit && screenRect.Contains(Input.mousePosition) )
		{
			Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			ShowCubePreview(mousePoint);

			Vector3 roundedPoint = CubeRound(cubeToDraw.transform.position);

			if ( Input.GetMouseButton(0) )
			{
				if ( roundedPoint != lastPosition )
				{
					AddCube(roundedPoint, cubeToDraw.transform.rotation, cubeToDraw);
					lastPosition = roundedPoint;
				}
			}
			else if (Input.GetMouseButton(1))
			{
				if ( roundedPoint != lastPosition )
				{
					RemoveCube(roundedPoint);
					lastPosition = roundedPoint;
				}
			}

		}
	}

	void OnEnable()
	{
		cubeToDraw = GameObject.Instantiate(cubeToDraw, Vector3.zero, Quaternion.identity) as LevelCubeObject;
	}

	void OnDisable()
	{
		Destroy(cubeToDraw);
	}

	#endregion
}
