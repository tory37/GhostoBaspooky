using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor;

[InitializeOnLoad]
public class GhostoLevelEditor : EditorWindow {

	//static GhostoLevelEditor ()
 //   {
 //       EditorApplication.update += EditLevel;
 //   }

	#region Public Interface

	public static GhostoLevelEditor Instance;

	[Serializable]
	private class WorldCubeProperties
	{
		public float cubeLength;
	}

	#endregion

	#region Editor Interface

	[SerializeField] private WorldCubeProperties pCube;

	public Transform previewCube;

	public EditorCube cubeToInstantiate;

	public Transform cubeParent;

	public Text coordinateDisplay;

	#endregion

	#region Private Functions

	private void ShowCubePreview()
	{
		float xPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
		float yPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

		//transparentCube.position = new Vector3(xPosition, yPosition, 0f);
		previewCube.position = CubeRound(xPosition, yPosition);
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

		coordinateDisplay.text = point.ToString();

		return point;
	}

	#endregion

	#region On GUI

	void Update()
	{
		ShowCubePreview();

		if ( Input.GetMouseButtonDown(0) )
			LevelContainer.AddCube(CubeRound(previewCube.transform.position.x, previewCube.transform.position.y), previewCube.transform.rotation, cubeToInstantiate, cubeParent);
	}

	#endregion
}