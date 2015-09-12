using UnityEngine;
using System.Collections;

public class Possession : MonoBehaviour
{

	#region Public Interface

	public static Possession Instance
	{
		get { return instance; }
	}
	[SerializeField] private static Possession instance;

	#endregion

}
