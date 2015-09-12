using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	#region Editor Interface

	public Transform levelCubesParent;

	#endregion

	#region Public Interface

	public static GameManager Instance { get { return instance; } }
	private static GameManager instance;

	/// <summary>
	/// A dictionary of all the cubes that make up the level,
	/// where the key is the position.  That way you can just
	/// input a position that's been sent througuh CubeRound 
	/// to see if there is something there
	/// </summary>
	public Dictionary<Vector3, LevelCubeObject> LevelCubes
	{
		get { return levelCubes; }
	}
	private Dictionary<Vector3, LevelCubeObject> levelCubes;

	#endregion

	#region Mono Methods

	private void Awake()
	{

		Instantiate();
		FillLevelCubesDict();
	}

	#endregion

	#region Private Functions

	private void  Instantiate()
	{
		instance = this;

		//Create the dictionary of cubes 
		levelCubes = new Dictionary<Vector3, LevelCubeObject>();
	}

	/// <summary>
	/// Goes through the transform that should be the parent
	/// of every cube in the game, then assigns it to the 
	/// levelCubes dictionary
	/// </summary>
	private void FillLevelCubesDict()
	{
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
