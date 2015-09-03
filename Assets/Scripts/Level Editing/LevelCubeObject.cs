using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelCubeObject : MonoBehaviour {


	#region Editor Interface

	[SerializeField] private CubeTypes cubeType;

	#endregion

	#region Public Interface

	public CubeTypes Type { get { return cubeType; } }

	#endregion 
}
