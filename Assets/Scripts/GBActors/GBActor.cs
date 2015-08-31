using UnityEngine;
using System.Collections;

/// <summary>
/// Base script that all in game actors inherit from.
/// </summary>
public abstract class GBActor : MonoBehaviour {

	//void Start()
	//{
	//	if ( GetComponent<GBActorStats>() == null )
	//	{
	//		#if UNITY_EDITOR
	//		Debug.LogWarning("Adding component GBActorStats to object.", gameObject);
	//		#endif
	//		gameObject.AddComponent<GBActorStats>();
	//	}
	//	if ( GetComponent<RotationVectors>() == null )
	//	{
	//		#if UNITY_EDITOR
	//		Debug.LogWarning("Adding component RotationVectors to object.", gameObject);
	//		#endif
	//		gameObject.AddComponent<RotationVectors>();
	//	}
	//}

}
