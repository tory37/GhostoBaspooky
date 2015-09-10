using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	#region Editor Interface

	public Transform levelCubesParent;

	#endregion

	#region Public Interface

	public static GameManager Instance;

	public Dictionary<Vector3, LevelCubeObject> LevelCubes
	{
		get { return levelCubes; }
	}
	private Dictionary<Vector3, LevelCubeObject> levelCubes;

	#endregion

	#region Mono Methods

	private void Awake()
	{
		Instance = this;

		levelCubes = new Dictionary<Vector3, LevelCubeObject>();

		foreach (Transform cube in levelCubesParent)
		{
			if ( cube.GetComponent<LevelCubeObject>() != null )
			{
				levelCubes.Add(cube.position, cube.GetComponent<LevelCubeObject>());
			}
			else
				Debug.Log("Child of levelCubesParent does not have a LevelCube component.  Only level cubes should be childed to that transform.", cube);
		}
	}

	#endregion
}
